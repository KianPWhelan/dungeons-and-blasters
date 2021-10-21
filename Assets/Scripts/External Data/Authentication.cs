using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class Authentication : MonoBehaviour
{
    FirebaseAuth auth = FirebaseAuth.DefaultInstance;

    public void CreateNewAccount(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {

        });
    }
}
