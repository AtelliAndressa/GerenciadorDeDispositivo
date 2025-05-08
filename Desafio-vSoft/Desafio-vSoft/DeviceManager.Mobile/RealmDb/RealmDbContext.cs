using Realms;

namespace DeviceManager.Mobile.RealmDb
{
    public class RealmDbContext
    {
        private static Realm _realmInstance;

        public static Realm GetInstance()
        {
            if (_realmInstance == null)
            {
                var config = new RealmConfiguration("dispositivos.realm")
                {
                    SchemaVersion = 1
                };

                _realmInstance = Realm.GetInstance(config);
            }

            return _realmInstance;
        }
    }
}
