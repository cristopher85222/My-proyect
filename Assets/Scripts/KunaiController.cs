using UnityEngine;
using UnityEngine.UI;

public class KunaiController: MonoBehaviour
{
    private string direccion = "Derecha";

    Rigidbody2D rb;
    SpriteRenderer sr;
    public GameObject Ninja;
    public int Puntos = 0;
    public float Scale = 1;

    void Start()
    {
        // Initialize the Kunai object
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        

        Destroy(this.gameObject, 5f);
    }

    void Update()
    {
        // Update the Kunai object
        this.transform.localScale = new Vector3(Scale, Scale);
        if (direccion == "Derecha")
        {
            rb.linearVelocityX = 15;
            sr.flipY = false;
            
        }
        else if (direccion == "Izquierda")
        {
            rb.linearVelocityX = -15;
            sr.flipY = true;
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision with the Kunai object
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            if (Ninja != null)
            {
                Ninja.GetComponent<PlayerController>().enemigosMuertosCount++;
                int n = Ninja.GetComponent<PlayerController>().enemigosMuertosCount;
                Ninja.GetComponent<PlayerController>().enemigosMuertosText.text = $"Enemigos muertos: {n}";
            }

            Destroy(this.gameObject);
            ZombieController enemy = collision.gameObject.GetComponent<ZombieController>();

            if(this.Puntos >= enemy.puntosVida)
            {
                Destroy(collision.gameObject);
            }
            else
            {
                enemy.puntosVida -= this.Puntos;
            }

                Debug.Log(Puntos);
        }
    }

    public void SetDirection(string direction)
    {
        this.direccion = direction;
    }
    
}
