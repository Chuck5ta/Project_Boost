using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;

    int currentScene = 0;
    int finalScene = 4; // 4 levels
    bool collisionsDisabled = true;



    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        finalScene = 4; // 4 levels
        collisionsDisabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentScene < SceneManager.sceneCountInBuildSettings-1)
            {
                currentScene++;
                SceneManager.LoadScene(currentScene);
            }
            else
            {
                currentScene = 0; // when we have done the last scene
                SceneManager.LoadScene(currentScene);
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentScene > 0)
            {
                currentScene--;
                SceneManager.LoadScene(currentScene);
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            print("Collison is " + collisionsDisabled);
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionsDisabled)
        {
            return;
        }
        currentScene = SceneManager.GetActiveScene().buildIndex;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default: //Dead
                StartDeathSequence();
                break;

        }
    }

    private void StartSuccessSequence()
    {
        currentScene++;
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadScene", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        currentScene = 0;
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadScene", levelLoadDelay);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(currentScene);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) // so it does not layer
            audioSource.PlayOneShot(mainEngine);
        mainEngineParticles.Play();
    }

    void RespondToRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            print("Cannot rotate in both directions at the same time!");
        }
        else if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rotationThisFrame);
        }
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; // take manual control of the rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}
