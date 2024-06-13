//using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
//public class PlayerController : MonoBehaviour
//{
//    // Configuración de las ruedas
//    public WheelCollider frontLeftWheel;
//    public WheelCollider frontRightWheel;
//    public WheelCollider rearLeftWheel;
//    public WheelCollider rearRightWheel;

//    // Variables para la física del carro
//    public float maxMotorTorque = 1500f; // Máximo torque del motor
//    public float maxSteeringAngle = 30f; // Ángulo máximo de dirección
//    public float brakeForce = 3000f; // Fuerza de frenado

//    // Componentes
//    private Rigidbody rb;

//    // Inicializa el componente Rigidbody
//    private void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.centerOfMass = new Vector3(0, -0.9f, 0); // Ajusta el centro de masa para mayor estabilidad
//    }

//    // Actualiza la física del carro cada frame
//    private void FixedUpdate()
//    {
//        // Captura las entradas del usuario
//        float motor = maxMotorTorque * Input.GetAxis("Vertical"); // Entrada de aceleración/frenado
//        float steering = maxSteeringAngle * Input.GetAxis("Horizontal"); // Entrada de dirección

//        // Aplica la dirección a las ruedas delanteras
//        frontLeftWheel.steerAngle = steering;
//        frontRightWheel.steerAngle = steering;

//        // Aplica el torque del motor a las ruedas traseras
//        rearLeftWheel.motorTorque = motor;
//        rearRightWheel.motorTorque = motor;

//        // Aplica fuerza de frenado si el jugador no está acelerando
//        ApplyBraking(Input.GetKey(KeyCode.Space));
//    }

//    // Aplica la fuerza de frenado a las cuatro ruedas
//    private void ApplyBraking(bool isBraking)
//    {
//        if (isBraking)
//        {
//            frontLeftWheel.brakeTorque = brakeForce;
//            frontRightWheel.brakeTorque = brakeForce;
//            rearLeftWheel.brakeTorque = brakeForce;
//            rearRightWheel.brakeTorque = brakeForce;
//        }
//        else
//        {
//            frontLeftWheel.brakeTorque = 0;
//            frontRightWheel.brakeTorque = 0;
//            rearLeftWheel.brakeTorque = 0;
//            rearRightWheel.brakeTorque = 0;
//        }
//    }

//    // Método opcional para visualizar la dirección de las ruedas en el editor
//    private void OnDrawGizmos()
//    {
//        DrawWheel(frontLeftWheel);
//        DrawWheel(frontRightWheel);
//        DrawWheel(rearLeftWheel);
//        DrawWheel(rearRightWheel);
//    }

//    // Dibuja la orientación de la rueda
//    private void DrawWheel(WheelCollider collider)
//    {
//        if (collider == null)
//            return;

//        var pos = collider.transform.position;
//        var rot = collider.transform.rotation;

//        Gizmos.DrawWireSphere(pos, 0.2f);
//        Gizmos.color = Color.red;
//        Gizmos.DrawRay(pos, rot * Vector3.forward * 0.5f);
//    }
//}
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // Declaracion de los coliders de las ruedas
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    // Declaracion de Variables para la física del carro
    public float maxMotorTorque = 1500f; //Maxima potencia del motor
    public float maxSteeringAngle = 30f; //Maximo angulo de giro
    public float brakeForce = 3000f; //Fuerza de freno de mano

    // Parámetros del Rigidbody
    public float carMass = 1500f; //Masa del carro
    public float drag = 0.05f; // Resistencia del aire
    public float angularDrag = 1f; //Resistencia para girar

    // Parámetros del Nitro
    public float nitroForce = 5000f; // Fuerza adicional aplicada durante el nitro
    public float nitroDuration = 2f; // Duración del nitro en segundos
    private float nitroTimer = 0f; // Temporizador para el nitro
    private bool isNitroActive = false; // Estado del nitro

    // Componentes
    private Rigidbody rb;

    // Material emisivo de luces traseras:
    public Material LucesFreno;
    private void Start() //CODIGO que se ejecuta al empezar el juego
    {
        rb = GetComponent<Rigidbody>(); //Obtener el Rigidbody del carro
        rb.mass = carMass; // FIJAR valores al Rigidbody del carro
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.centerOfMass = new Vector3(0, -0.9f, 0); // Ajusta el centro de masa para mayor estabilidad
    }

    private void FixedUpdate() // CODIGO QUE SE EJECUTA 50 veces por segundo
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical"); // Multiplicar la potencia del carro por la entrada del telefono
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal"); //Multiplicar la potencia del carro por el angulo de giro

        frontLeftWheel.steerAngle = steering; //Asignar el giro a las llantas
        frontRightWheel.steerAngle = steering; //Asignar el giro a las llantas

        if (isNitroActive) //Verificar si se ha activado el nitro
        {
            // Aplica fuerza de nitro al Rigidbody
            rb.AddForce(transform.forward * nitroForce, ForceMode.Acceleration);

            // Actualiza el temporizador de nitro
            nitroTimer -= Time.deltaTime; // Time.deltatime es el tiempo real en segundos

            // Desactiva el nitro cuando el temporizador llega a cero
            if (nitroTimer <= 0)
            {
                isNitroActive = false;
            }
        }
        else
        {   //Aplicar la potencia multiplicada al motor
            rearLeftWheel.motorTorque = motor;
            rearRightWheel.motorTorque = motor;
        }

        ApplyBraking(Input.GetKey(KeyCode.Space)); //LLamar a la funcion de freno cuando se preciona el boton

        // Activa el nitro cuando se presiona "Q"
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateNitro(); //Funcion para el nitro
        }
    }

    private void ApplyBraking(bool isBraking) //Funcion de freno
    {
        if (isBraking)
        {   //Aplicar a las llantas el frenado
            LucesFreno.EnableKeyword("_EMISSION");
            frontLeftWheel.brakeTorque = brakeForce;
            frontRightWheel.brakeTorque = brakeForce;
            rearLeftWheel.brakeTorque = brakeForce;
            rearRightWheel.brakeTorque = brakeForce;
        }
        else
        {   //No aplicar frenado a las llantas
            LucesFreno.DisableKeyword("_EMISSION");
            frontLeftWheel.brakeTorque = 0;
            frontRightWheel.brakeTorque = 0;
            rearLeftWheel.brakeTorque = 0;
            rearRightWheel.brakeTorque = 0;
        }
    }

    private void ActivateNitro()
    {   //Activar el nitro y establecer el timer
        isNitroActive = true;
        nitroTimer = nitroDuration;
    }
}


