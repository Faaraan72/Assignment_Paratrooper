using UnityEngine;

public class Paratrooper : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float landThreshold = 0.1f;
    private Vector3 targetLandingZone;
   // private GameController shooter;

    
    private Animator anim;
    private bool isLeft;
    public void Initialize(bool left)
    {
        anim = GetComponent<Animator>();
        anim.SetBool("fall", true);
        Invoke(nameof(openParachute), 0.5f);
        isLeft = left;
        
    }
    void Update()
    {
       //testing Purpose
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    closeParachute();
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    openParachute();
        //}
    }

    void Land()
    {
        anim.SetBool("fall", false);
        transform.GetChild(0).gameObject.SetActive(false);
        // add to list when landed
        if (isLeft) { GameController.instance.leftPara.Add(gameObject); }
        else { GameController.instance.rightPara.Add(gameObject); GetComponent<SpriteRenderer>().flipX = true; }
         GameController.instance.checkGameover();
    }
     public void openParachute()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Rigidbody2D>().drag = 8;
    }
      public void closeParachute()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Rigidbody2D>().drag = 1;

    }

    //trigger for detecting Ground hit and land
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            float landingVelocity = collision.relativeVelocity.magnitude;
            Debug.Log("Landing Velocity: " + landingVelocity);

            if (landingVelocity > 5f) 
            {
                GameController.coin += 10;
                //Transform temp = transform;
               GameController.instance.BloodEffect(transform.position);
                Destroy(gameObject);

            }
            else
            {
                Land();
            }
        }
    }
}
