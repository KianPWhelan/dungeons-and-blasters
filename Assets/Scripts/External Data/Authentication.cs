using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class Authentication : MonoBehaviour
{
    [SerializeField]
    private InputField email;

    [SerializeField]
    private InputField password;

    [SerializeField]
    private Text loginInfo;

    private FirebaseAuth auth;

    private FirebaseUser user;

    public Button registerButton;
    public Button loginButton;
    public Button logoutButton;

    public string displayName;

    public string emailAddress;

    public void Awake()
    {
        InitializeFirebase();
    }

    public virtual void Start()
    {
        DontDestroyOnLoad(gameObject);

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Dependencies found");
                Awake();
            }
            else
            {
                Debug.LogError(
                "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public void Register()
    {
        CreateNewAccount(email.text, password.text);
    }

    public void Login()
    {
        SignInToAccount(email.text, password.text);
    }

    public void LogOut()
    {
        auth.SignOut();
        user.DeleteAsync();
    }

    private void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                loginInfo.text = "Not Logged In";
                registerButton.gameObject.SetActive(true);
                loginButton.gameObject.SetActive(true);
                logoutButton.gameObject.SetActive(false);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                emailAddress = user.Email ?? "";
                Debug.Log(emailAddress);
                loginInfo.text = "Logged In As: " + emailAddress;
                registerButton.gameObject.SetActive(false);
                loginButton.gameObject.SetActive(false);
                logoutButton.gameObject.SetActive(true);
            }
        }
    }
    private void CreateNewAccount(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    private void SignInToAccount(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
}
