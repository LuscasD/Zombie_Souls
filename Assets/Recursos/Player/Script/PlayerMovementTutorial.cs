using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerMovementTutorial : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4;
    public float rotateSpeed = 15;
    public int gravidade = 25;

    float velocityY;

    CharacterController characterController;

    Vector2 moveInput;
    Vector3 direction;

    Transform cam;


    [Header("Dodge")]
    [SerializeField] AnimationCurve dodgeCurve;
    bool isdodgin;
    public float dodgeTimer;
    float dodge_coolDown;
    public float dodgeDistance;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    Vector3 moveDirection;

    Rigidbody rb;

    public Animator anim;

   public bool lockMovement;

    public GameObject playerModel;


    private void Start()
    {
        cam = Camera.main.transform;

        characterController = GetComponent<CharacterController>();

        anim = GetComponent<Animator>();

        Keyframe dodge_lastFrame = dodgeCurve[dodgeCurve.length - 1];

        dodgeTimer = dodge_lastFrame.time;
    }

    void Update()
    {
        RecordControls();

        if (!isdodgin) MovePlayer();

       if(!lockMovement) PlayerRotation();

        if (dodge_coolDown > 0) dodge_coolDown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (dodge_coolDown > 0) return;

            if (direction.magnitude != 0)
            {

                StartCoroutine(Dodge()); //Only if the character is moving, dodging is allowed.
            }
        }
    }


        void RecordControls()
        {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 forward = cam.forward;

        Vector3 right = cam.right;

        forward.y = 0;

        right.y = 0;

        forward.Normalize();

        right.Normalize();

        direction = (right * moveInput.x + forward * moveInput.y).normalized;

        anim.SetFloat("Walk", direction.magnitude, 0.1f, Time.deltaTime);
    }

        void MovePlayer()
        {
        velocityY -= Time.deltaTime * gravidade;

        velocityY = Mathf.Clamp(velocityY, -10, 10);

        Vector3 fallVelocity = Vector3.up * velocityY;

        Vector3 velocity = (direction * moveSpeed) + fallVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

        void PlayerRotation()
        {
        if (direction.magnitude == 0) return;

        float rs = rotateSpeed;

        if (isdodgin) rs = 3;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rs * Time.deltaTime);

    }


        IEnumerator Dodge()
        {
            anim.SetTrigger("Dodge");

            isdodgin = true;

            float timer = 0;

            dodge_coolDown = dodgeTimer + 0.25f;

            while (timer < dodgeTimer)
            {

                float speed = dodgeCurve.Evaluate(timer);


                //Adicionando direção e gravidade.
                Vector3 dir = (playerModel.transform.forward * speed * dodgeDistance) + (Vector3.up * velocityY);
                characterController.Move(dir * Time.deltaTime);

                timer += Time.deltaTime;

                yield return null;

            }
           isdodgin = false;

        }
    }
