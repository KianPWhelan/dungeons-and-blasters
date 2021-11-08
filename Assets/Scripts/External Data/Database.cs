using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Newtonsoft.Json;
using Firebase.Extensions;
using System.Threading.Tasks;

public class Database : MonoBehaviour
{
    public DatabaseReference reference;

    // Start is called before the first frame update
    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        reference = FirebaseDatabase.GetInstance("https://dmshooter-b881e-default-rtdb.firebaseio.com").RootReference;
        Debug.Log("Reference:");
        Debug.Log(reference);

        //AddCategory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveRoomUnderCurrentUser(string roomJson, string name)
    {
        Debug.Log(name);
        reference.Child("Users").Child("testgmailcom").Child("Rooms").Child(name).SetRawJsonValueAsync(roomJson);
    }

    public async Task LoadRoomFromCurrentUserByName(string name, StringHolder json)
    {
        await reference.Child("Users").Child("testgmailcom").Child("Rooms").Child(name).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                json.value = snapshot.GetRawJsonValue();
                Debug.Log("Here");
                Debug.Log(json.value);
            };
        });

        Debug.Log(json.value);
    }
}

public class StringHolder
{
    public string value;
}
