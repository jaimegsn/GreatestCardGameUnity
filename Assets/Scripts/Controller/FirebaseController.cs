using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using Newtonsoft.Json;

public class FirebaseController : MonoBehaviour
{
    private DependencyStatus dependencyStatus;
    public DatabaseReference database;
    public bool isFirebaseInitialized;
    List<string> data = new List<string>();
    public List<Matchmaking> matchList = new List<Matchmaking>();

    MatchmakingController matchmakingController;

    // Start is called before the first frame update
    void Awake()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        matchmakingController = GetComponent<MatchmakingController>();
        
        AuthenticateWithAnonymous();
        CreateDatabase();
    }
    void AuthenticateWithAnonymous()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                return;
            }
            if (task.IsFaulted)
            {
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result.User;
        });
    }

    void CreateDatabase()
    {

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.Create();
                isFirebaseInitialized = true;
                database = FirebaseDatabase.GetInstance(app).GetReference("greatestunity");
                matchmakingController.enabled = true;
                GetMatchMaking();
            }
            else
            {
                return;
            }
        });
    }

    public void GetMatchMaking()
    {
        database.Child("matchmaking")
        .ValueChanged += (object sender, ValueChangedEventArgs args) =>
            {
                if (args.DatabaseError != null)
                {
                    return;
                }
                else
                {
                    matchList.Clear();
                    foreach (var item in args.Snapshot.Children)
                    {
                        data.Add(item.Key);
                        var matchJson = JsonConvert.DeserializeObject<Matchmaking>(item.GetRawJsonValue());
                        matchList.Add(matchJson);
                    }
                    return;
                }
            };
    }
}
