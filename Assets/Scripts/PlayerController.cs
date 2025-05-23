using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 10f;
    public float fuerzaSalto = 12.5f;
    
    public GameObject kunaiPrefab;
    public int kunaisDisponibles = 5;

    public Transform groundCheck;
    public LayerMask groundLayer;


    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    private bool isGrounded = true;
    private string direccion = "Derecha";
    private bool puedeMoverseVerticalMente = false;
    private float defaultGravityScale = 1f;
    private bool puedeSaltar = true;
    private bool puedeLanzarKunai = true;


    [Header("Parámetros de salto")]
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    public Text enemigosMuertosText;
    public int enemigosMuertosCount = 0;

    public Text puntosVidaText;
    public int puntosVidaCount = 3;

    private DateTime? timeStart;
    private DateTime? timeEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Iniciando PlayerController");

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        defaultGravityScale = rb.gravityScale;

        enemigosMuertosText.text = $"Enemigos muertos: {enemigosMuertosCount}";
        puntosVidaText.text = $"Vidas: {puntosVidaCount}";

    }

    // Update is called once per frame
    void Update()
    {
        SetupMoverseHorizontal();
        SetupMoverseVertical();
        SetupSalto();
        SetUpLanzarKunai();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();
            //Debug.Log($"Colision con Enemigo: ${zombie.puntosVida}");
            puntosVidaCount--;
            puntosVidaText.text = $"Vidas: {puntosVidaCount}";
            //Destroy(collision.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log($"Trigger con: {other.gameObject.name}");
        if (other.gameObject.name == "Muro") {
            puedeMoverseVerticalMente = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log($"Trigger con: {other.gameObject.name}");
        if (other.gameObject.name == "Muro") {
            puedeMoverseVerticalMente = false;
            rb.gravityScale = defaultGravityScale;
        }
    }


    void SetupMoverseHorizontal() {
        
        //Debug.Log($"isGrounded: {isGrounded}, {rb.linearVelocityY}");
        if (isGrounded && rb.linearVelocityY == 0) {
            animator.SetInteger("Estado", 0);
        }

        rb.linearVelocityX = 0;
        if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocityX = velocidad;
            sr.flipX = false;
            direccion = "Derecha";
            if (isGrounded && rb.linearVelocityY == 0)
                animator.SetInteger("Estado", 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocityX = -velocidad;
            sr.flipX = true;
            direccion = "Izquierda";
            if (isGrounded && rb.linearVelocityY == 0)
                animator.SetInteger("Estado", 1);
        }
    }

    void SetupMoverseVertical() {
        
        if (!puedeMoverseVerticalMente) return;
        rb.gravityScale = 0;
        rb.linearVelocityY = 0;
        if (Input.GetKey(KeyCode.W))
        {
            rb.linearVelocityY = velocidad;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.linearVelocityY = -velocidad;
        }
    }

    void SetupSalto() {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // --- Coyote Time ---
        if (isGrounded) {
            coyoteTimeCounter = coyoteTime;
        }
        else {
            coyoteTimeCounter -= Time.deltaTime;
            if (rb.linearVelocityY > 5f)
                animator.SetInteger("Estado", 3);
            
        }

        


        // --- Jump Buffer ---
        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        }
        else
            jumpBufferCounter -= Time.deltaTime;

        // --- Ejecutar salto ---
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocityY = jumpForce;
            jumpBufferCounter = 0f;
        }

        // --- Ajuste de gravedad para caída más rápida ---
        if (rb.linearVelocityY < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocityY > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
            
        }
    }

    void SetUpLanzarKunai() {
        if (!puedeLanzarKunai || kunaisDisponibles <= 0) return;
        /*if (Input.GetKeyUp(KeyCode.Q))
        {
            GameObject kunai = Instantiate(kunaiPrefab, transform.position, Quaternion.Euler(0, 0, -90));
            kunai.GetComponent<KunaiController>().SetDirection(direccion);
            kunaisDisponibles -= 1;
            kunai.GetComponent<KunaiController>().Ninja = this.gameObject;
        }*/


        if (Input.GetKeyDown(KeyCode.Q))
            timeStart = DateTime.Now;
        if(Input.GetKeyUp(KeyCode.Q))
            timeEnd = DateTime.Now;

        if(timeStart == null || timeEnd == null)
            return;

        var seconds = (timeEnd - timeStart).Value.TotalSeconds;
        Debug.Log(seconds);
       

        if (seconds <= 1d)
        {
            this.kunaiPrefab.GetComponent<KunaiController>().Puntos = 1;
            this.kunaiPrefab.GetComponent<KunaiController>().Scale = 1;
        }
        else if(seconds > 1d && seconds < 3d)
        {
            this.kunaiPrefab.GetComponent<KunaiController>().Puntos = 2;
            this.kunaiPrefab.GetComponent<KunaiController>().Scale = 2;
        }
        else if(seconds >= 3d)
        {
            this.kunaiPrefab.GetComponent<KunaiController>().Puntos = 3;
            this.kunaiPrefab.GetComponent<KunaiController>().Scale = 3;
        }

        this.kunaiPrefab.GetComponent<KunaiController>().SetDirection(direccion);
        this.kunaiPrefab.GetComponent<KunaiController>().Ninja = this.gameObject;
        GameObject kunai = Instantiate(kunaiPrefab, transform.position, Quaternion.Euler(0, 0, -90));
        //kunaisDisponibles -= 1;

        timeEnd = null;
        timeStart = null;
    }

    // Visualiza el groundCheck en el editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}
