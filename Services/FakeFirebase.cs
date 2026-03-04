using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Firebase.Auth
{
    public class FirebaseUser
    {
        public string Uid { get; set; }
        public string Email { get; set; }
    }

    public class AuthResult
    {
        public FirebaseUser User { get; set; }
    }

    public class CrossFirebaseAuth
    {
        public static readonly FakeAuth Current = new FakeAuth();
    }

    public class FakeAuth
    {
        public FirebaseUser CurrentUser { get; private set; }

        public Task<AuthResult> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            // Simple in-memory auth: any non-empty credentials succeed
            var user = new FirebaseUser { Uid = Guid.NewGuid().ToString(), Email = email };
            CurrentUser = user;
            return Task.FromResult(new AuthResult { User = user });
        }

        public Task<AuthResult> CreateUserWithEmailAndPasswordAsync(string email, string password)
        {
            // Create user similarly
            var user = new FirebaseUser { Uid = Guid.NewGuid().ToString(), Email = email };
            CurrentUser = user;
            return Task.FromResult(new AuthResult { User = user });
        }
    }
}

namespace Plugin.Firebase.CloudMessaging
{
    public class CrossFirebaseCloudMessaging
    {
        public static readonly FakeCloudMessaging Current = new FakeCloudMessaging();
    }

    public class FakeCloudMessaging
    {
        public Task<string> GetTokenAsync()
        {
            return Task.FromResult(string.Empty);
        }

        public Task SubscribeToTopicAsync(string topic)
        {
            return Task.CompletedTask;
        }
    }
}

namespace Plugin.Firebase.Firestore
{
    // Compatibility wrapper types used in the app
    public class DocumentReferenceInfo { public string Id { get; set; } }

    public class DocumentSnapshot
    {
        public string Id { get; set; }
        public bool Exists { get; set; }
        private readonly Dictionary<string, object> _data;
        public object Data { get; }
        public DocumentReferenceInfo Reference { get; }

        public DocumentSnapshot(string id, Dictionary<string, object> data)
        {
            Id = id;
            _data = data ?? new Dictionary<string, object>();
            Exists = data != null;
            Data = _data == null ? null : new Dictionary<string, object>(_data);
            Reference = new DocumentReferenceInfo { Id = id };
        }

        public object Get(string key)
        {
            if (_data != null && _data.TryGetValue(key, out var v)) return v;
            return null;
        }

        public Dictionary<string, object> ToDictionary() => _data == null ? new Dictionary<string, object>() : new Dictionary<string, object>(_data);
    }

    public class QueryDocumentSnapshot : DocumentSnapshot
    {
        public QueryDocumentSnapshot(string id, Dictionary<string, object> data) : base(id, data) { }
    }

    public class QuerySnapshot
    {
        public List<QueryDocumentSnapshot> Documents { get; set; } = new List<QueryDocumentSnapshot>();
    }

    // Low-level storage
    public class CrossFirebaseFirestore
    {
        public static readonly FakeFirestore Current = new FakeFirestore();
    }

    public class FakeFirestore
    {
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _db = new();

        // Old API compatibility methods used across the app
        public CollectionWrapper GetCollection(string name) => new CollectionWrapper(name, _db);

        public DocumentWrapper GetDocument(string path)
        {
            // path like "collection/docId"
            var parts = path?.Split('/');
            if (parts == null || parts.Length != 2) return new DocumentWrapper(null, null, _db);
            return new DocumentWrapper(parts[0], parts[1], _db);
        }

        // Newer Collection(...) API also supported
        public CollectionReference Collection(string name) => new CollectionReference(name, _db);
    }

    // Wrapper that exposes GetDocumentsAsync, GetDocument, DeleteDocumentAsync, UpdateAsync, etc.
    public class CollectionWrapper
    {
        private readonly string _collection;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _db;

        public CollectionWrapper(string collection, Dictionary<string, Dictionary<string, Dictionary<string, object>>> db)
        {
            _collection = collection;
            _db = db;
        }

        public async Task<QuerySnapshot> GetDocumentsAsync<T>()
        {
            var snap = new QuerySnapshot();
            if (_db.TryGetValue(_collection, out var coll))
            {
                foreach (var kv in coll)
                {
                    snap.Documents.Add(new QueryDocumentSnapshot(kv.Key, new Dictionary<string, object>(kv.Value)));
                }
            }
            return await Task.FromResult(snap);
        }

        public DocumentWrapper GetDocument(string id)
        {
            return new DocumentWrapper(_collection, id, _db);
        }
    }

    public class DocumentWrapper
    {
        private readonly string _collection;
        private readonly string _id;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _db;

        public DocumentWrapper(string collection, string id, Dictionary<string, Dictionary<string, Dictionary<string, object>>> db)
        {
            _collection = collection;
            _id = id;
            _db = db;
        }

        public async Task<QueryDocumentSnapshot> GetDocumentSnapshotAsync<T>()
        {
            if (_collection == null || _id == null) return await Task.FromResult(new QueryDocumentSnapshot("", null));
            if (_db.TryGetValue(_collection, out var coll) && coll.TryGetValue(_id, out var data))
            {
                return await Task.FromResult(new QueryDocumentSnapshot(_id, new Dictionary<string, object>(data)));
            }
            return await Task.FromResult(new QueryDocumentSnapshot(_id, null));
        }

        public async Task DeleteDocumentAsync()
        {
            if (_collection == null || _id == null) return;
            if (_db.TryGetValue(_collection, out var coll)) coll.Remove(_id);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Dictionary<string, object> updates)
        {
            if (_collection == null || _id == null) return;
            if (!_db.TryGetValue(_collection, out var coll))
            {
                coll = new Dictionary<string, Dictionary<string, object>>();
                _db[_collection] = coll;
            }
            if (!coll.TryGetValue(_id, out var existing))
            {
                existing = new Dictionary<string, object>();
                coll[_id] = existing;
            }
            foreach (var kv in updates)
            {
                existing[kv.Key] = kv.Value;
            }
            await Task.CompletedTask;
        }

        public Task UpdateDataAsync(Dictionary<object, object> updates)
        {
            // Some code uses object keys; convert to string keys
            var dict = new Dictionary<string, object>();
            foreach (var kv in updates)
            {
                dict[kv.Key?.ToString() ?? ""] = kv.Value;
            }
            return UpdateAsync(dict);
        }

        public Task SetAsync(Dictionary<string, object> data)
        {
            if (_collection == null || _id == null) return Task.CompletedTask;
            if (!_db.TryGetValue(_collection, out var coll))
            {
                coll = new Dictionary<string, Dictionary<string, object>>();
                _db[_collection] = coll;
            }
            coll[_id] = new Dictionary<string, object>(data);
            return Task.CompletedTask;
        }

        // Compatibility alias used in some pages
        public Task SetDataAsync(Dictionary<string, object> data) => SetAsync(data);
    }

    // Also provide CollectionReference and DocumentReference compatible with some usages
    public class DocumentReference
    {
        private readonly string _collection;
        private readonly string _id;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _db;

        public DocumentReference(string collection, string id, Dictionary<string, Dictionary<string, Dictionary<string, object>>> db)
        {
            _collection = collection;
            _id = id;
            _db = db;
        }

        public Task<DocumentSnapshot> GetAsync()
        {
            if (_db.TryGetValue(_collection, out var coll) && coll.TryGetValue(_id, out var data))
            {
                return Task.FromResult(new DocumentSnapshot(_id, new Dictionary<string, object>(data)));
            }
            return Task.FromResult(new DocumentSnapshot(_id, null as Dictionary<string, object>));
        }

        public Task SetAsync(Dictionary<string, object> data)
        {
            if (!_db.TryGetValue(_collection, out var coll))
            {
                coll = new Dictionary<string, Dictionary<string, object>>();
                _db[_collection] = coll;
            }
            coll[_id] = new Dictionary<string, object>(data);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Dictionary<string, object> updates)
        {
            if (!_db.TryGetValue(_collection, out var coll))
            {
                coll = new Dictionary<string, Dictionary<string, object>>();
                _db[_collection] = coll;
            }
            if (!coll.TryGetValue(_id, out var existing))
            {
                existing = new Dictionary<string, object>();
                coll[_id] = existing;
            }
            foreach (var kv in updates)
            {
                existing[kv.Key] = kv.Value;
            }
            return Task.CompletedTask;
        }
    }

    public class CollectionReference
    {
        private readonly string _collection;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _db;

        public CollectionReference(string collection, Dictionary<string, Dictionary<string, Dictionary<string, object>>> db)
        {
            _collection = collection;
            _db = db;
        }

        public DocumentReference Document(string id)
        {
            return new DocumentReference(_collection, id, _db);
        }

        public Task AddAsync(Dictionary<string, object> data)
        {
            if (!_db.TryGetValue(_collection, out var coll))
            {
                coll = new Dictionary<string, Dictionary<string, object>>();
                _db[_collection] = coll;
            }
            var id = Guid.NewGuid().ToString();
            coll[id] = new Dictionary<string, object>(data);
            return Task.CompletedTask;
        }

        public Task<QuerySnapshot> GetDocumentsAsync()
        {
            var snapshot = new QuerySnapshot();
            if (_db.TryGetValue(_collection, out var coll))
            {
                foreach (var kv in coll)
                {
                    snapshot.Documents.Add(new QueryDocumentSnapshot(kv.Key, new Dictionary<string, object>(kv.Value)));
                }
            }
            return Task.FromResult(snapshot);
        }

        public Query WhereEqualTo(string key, object value)
        {
            return new Query(_collection, _db).WhereEqualTo(key, value);
        }

        public Task<QuerySnapshot> GetAsync()
        {
            var snapshot = new QuerySnapshot();
            if (_db.TryGetValue(_collection, out var coll))
            {
                foreach (var kv in coll)
                {
                    snapshot.Documents.Add(new QueryDocumentSnapshot(kv.Key, new Dictionary<string, object>(kv.Value)));
                }
            }
            return Task.FromResult(snapshot);
        }
    }

    public class Query
    {
        private readonly string _collection;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _db;
        private string _key;
        private object _value;

        public Query(string collection, Dictionary<string, Dictionary<string, Dictionary<string, object>>> db)
        {
            _collection = collection;
            _db = db;
        }

        public Query WhereEqualTo(string key, object value)
        {
            _key = key;
            _value = value;
            return this;
        }

        public Task<QuerySnapshot> GetAsync()
        {
            var snapshot = new QuerySnapshot();
            if (_db.TryGetValue(_collection, out var coll))
            {
                foreach (var kv in coll)
                {
                    var contains = false;
                    if (_key == null) contains = true;
                    else if (kv.Value.TryGetValue(_key, out var v) && object.Equals(v, _value)) contains = true;

                    if (contains)
                    {
                        snapshot.Documents.Add(new QueryDocumentSnapshot(kv.Key, new Dictionary<string, object>(kv.Value)));
                    }
                }
            }
            return Task.FromResult(snapshot);
        }
    }
}