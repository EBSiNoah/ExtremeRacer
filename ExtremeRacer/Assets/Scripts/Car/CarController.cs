using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 프로메테우스 카 컨트롤러 기반임 여기에 에제리얼 카 컨트롤러에서 가져온 거 합칠꺼임
/// 
/// </summary>
namespace songkim
{
    public class CarController : MonoBehaviour
    {
        //[Header("CAR SETUP")]
        [Space(10)]
        [Range(20, 190)]
        public int maxSpeed = 90; // 자동차가 도달할 수 있는 최대 속도(km/h).
        [Range(10, 120)]
        public int maxReverseSpeed = 45; // 자동차가 후진할 때 도달할 수 있는 최대 속도(km/h).
        [Range(1, 10)]
        public int accelerationMultiplier = 2; // 자동차 가속도 수치. 1 is a slow acceleration and 10 is the fastest.
        [Space(10)]
        [Range(10, 45)]
        public int maxSteeringAngle = 27; // 차량 핸들(바퀴) 돌아가는 최대 각도
        [Range(0.1f, 1f)]
        public float steeringSpeed = 0.5f; // 핸들 돌리는 속도
        [Space(10)]
        [Range(100, 600)]
        public int brakeForce = 350; // 브레이크 힘 강도
        [Range(1, 10)]
        public int decelerationMultiplier = 2; // 엑셀 떼면 자동차가 감속하는 수치
        [Range(1, 10)]
        public int handbrakeDriftMultiplier = 5; // 핸드브레이크 사용 시 자동차가 그립을 잃는 수치
        [Space(10)]
        public Vector3 bodyMassCenter; // 차랴으이 질량 중심
                                       // 차량 오브젝트의 x = 0, z = 0을 추천. 높이인 y축을 선택가능,
                                       // 갚이 높아질 수록 차량이 불안정해짐
                                       // 보통 0 - 1.5 사이를 추천

        //WHEELS

        //[Header("WHEELS")]

        /*
        차량의 바퀴 데이터를 저장. 차량의 메시만 있는 오브젝트와 휠 콜라이더가 있는 오브젝트가 필요함
        두 데이터를 같은 게임 오브젝트에 등록 하지말고 반드시 분리해서 저장할 것
        */
        public GameObject frontLeftMesh;
        public WheelCollider frontLeftCollider;
        [Space(10)]
        public GameObject frontRightMesh;
        public WheelCollider frontRightCollider;
        [Space(10)]
        public GameObject rearLeftMesh;
        public WheelCollider rearLeftCollider;
        [Space(10)]
        public GameObject rearRightMesh;
        public WheelCollider rearRightCollider;

        //PARTICLE SYSTEMS

        [Space(20)]
        //[Header("EFFECTS")]
        [Space(10)]
        // 파티클 시스템을 사용할지 말지 변수
        public bool useEffects = false;

        // 자동차 드리프트시 차량 바퀴 연기
        public ParticleSystem RLWParticleSystem;
        public ParticleSystem RRWParticleSystem;

        [Space(10)]
        // 차량이 트랙션을 잃었을 때 나오는 바퀴 자국 
        public TrailRenderer RLWTireSkid;
        public TrailRenderer RRWTireSkid;

        //SPEED TEXT (UI)

        [Space(20)]
        //[Header("UI")]
        [Space(10)]
        // UI 사용 할지 말지
        public bool useUI = false;
        public Text carSpeedText; // 차량 속도 UI

        //SOUNDS

        [Space(20)]
        //[Header("Sounds")]
        [Space(10)]
        // 차량 엔진 사운드나 차량 미끄러지는 사운드
        public bool useSounds = false;
        public AudioSource carEngineSound;
        public AudioSource tireScreechSound;
        float initialCarEngineSoundPitch; // 차량 엔진음 기본 높이 설정

        //CONTROLS

        [Space(20)]
        //[Header("CONTROLS")]
        [Space(10)]
        // 모바일 기기 대응
        public bool useTouchControls = false;
        public GameObject throttleButton;
        PrometeoTouchInput throttlePTI;
        public GameObject reverseButton;
        PrometeoTouchInput reversePTI;
        public GameObject turnRightButton;
        PrometeoTouchInput turnRightPTI;
        public GameObject turnLeftButton;
        PrometeoTouchInput turnLeftPTI;
        public GameObject handbrakeButton;
        PrometeoTouchInput handbrakePTI;

        //CAR DATA

        [HideInInspector]
        public float carSpeed; // 차량 속도
        [HideInInspector]
        public bool isDrifting; // 현재 드리프트 중인지 여부
        [HideInInspector]
        public bool isTractionLocked; // 자동차의 트랙션이 잠겨 있는지 여부

        //PRIVATE VARIABLES

        /*
        중요: 다음 변수들은 스크립트에 의해 자동으로 변하는 것을 제외한 직접적인 수정 금지
        */
        Rigidbody carRigidbody; // 자동차 리지드바디
        float steeringAxis; // 핸들 수치 좌에서 우. -1 에서 1까지.
        float throttleAxis; // 쓰로틀 수치 후진에서 전진까지. -1 에서 1까지
        float driftingAxis;
        float localVelocityZ;
        float localVelocityX;
        bool deceleratingCar;
        bool touchControlsSetup = false;

        /*
        다음 변수는 휠 측면 마찰에 대한 정보를 저장하는데 사용됨. 이런 값을 변경해서 자동차를 드리프트하게 만듬
        (such as extremumSlip,extremumValue, asymptoteSlip, asymptoteValue and stiffness). 
        */
        WheelFrictionCurve FLwheelFriction;
        float FLWextremumSlip;
        WheelFrictionCurve FRwheelFriction;
        float FRWextremumSlip;
        WheelFrictionCurve RLwheelFriction;
        float RLWextremumSlip;
        WheelFrictionCurve RRwheelFriction;
        float RRWextremumSlip;

        void Start()
        {
            // 차량 초기 설정
            carRigidbody = gameObject.GetComponent<Rigidbody>();
            carRigidbody.centerOfMass = bodyMassCenter;

            // 드리프트 초기값 설정, 바퀴에 기본 마찰값 설정
            FLwheelFriction = new WheelFrictionCurve();
            FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
            FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
            FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
            FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
            FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
            FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
            FRwheelFriction = new WheelFrictionCurve();
            FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
            FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
            FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
            FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
            FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
            FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
            RLwheelFriction = new WheelFrictionCurve();
            RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
            RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
            RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
            RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
            RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
            RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
            RRwheelFriction = new WheelFrictionCurve();
            RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
            RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
            RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
            RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
            RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
            RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

            // 차량 엔진음 기본 높이 설정
            if (carEngineSound != null)
            {
                initialCarEngineSoundPitch = carEngineSound.pitch;
            }

            // 0.1초마다 차량 속도에 따른 UI와 소리를 제어함
            if (useUI)
            {
                InvokeRepeating("CarSpeedUI", 0f, 0.1f);
            }
            else if (!useUI)
            {
                if (carSpeedText != null)
                {
                    carSpeedText.text = "0";
                }
            }

            if (useSounds)
            {
                InvokeRepeating("CarSounds", 0f, 0.1f);
            }
            else if (!useSounds)
            {
                if (carEngineSound != null)
                {
                    carEngineSound.Stop();
                }
                if (tireScreechSound != null)
                {
                    tireScreechSound.Stop();
                }
            }

            if (!useEffects)
            {
                if (RLWParticleSystem != null)
                {
                    RLWParticleSystem.Stop();
                }
                if (RRWParticleSystem != null)
                {
                    RRWParticleSystem.Stop();
                }
                if (RLWTireSkid != null)
                {
                    RLWTireSkid.emitting = false;
                }
                if (RRWTireSkid != null)
                {
                    RRWTireSkid.emitting = false;
                }
            }

            if (useTouchControls)
            {
                if (throttleButton != null && reverseButton != null &&
                turnRightButton != null && turnLeftButton != null
                && handbrakeButton != null)
                {

                    throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
                    reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
                    turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
                    turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
                    handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
                    touchControlsSetup = true;

                }
                else
                {
                    String ex = "Touch controls are not completely set up. You must drag and drop your scene buttons in the" +
                    " CarController component.";
                    Debug.LogWarning(ex);
                }
            }

        }

        void Update()
        {
            //CAR DATA

            // 차량 속도 
            carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
            // 차량의 x축 속도를 저장 (드리프트 여부 )
            localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
            // 차량의 z축 속도 저장 (전진 후진 여부)
            localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

            //CAR PHYSICS

            /*
            입력 제어 -> 나중에 input asset으로 변경하기
            */
            if (useTouchControls && touchControlsSetup)
            {

                if (throttlePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    GoForward();
                }
                if (reversePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    GoReverse();
                }

                if (turnLeftPTI.buttonPressed)
                {
                    TurnLeft();
                }
                if (turnRightPTI.buttonPressed)
                {
                    TurnRight();
                }
                if (handbrakePTI.buttonPressed)
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    Handbrake();
                }
                if (!handbrakePTI.buttonPressed)
                {
                    RecoverTraction();
                }
                if ((!throttlePTI.buttonPressed && !reversePTI.buttonPressed))
                {
                    ThrottleOff();
                }
                if ((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed && !deceleratingCar)
                {
                    InvokeRepeating("DecelerateCar", 0f, 0.1f);
                    deceleratingCar = true;
                }
                if (!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f)
                {
                    ResetSteeringAngle();
                }

            }
            else
            {

                if (Input.GetKey(KeyCode.W))
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    GoForward();
                }
                if (Input.GetKey(KeyCode.S))
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    GoReverse();
                }

                if (Input.GetKey(KeyCode.A))
                {
                    TurnLeft();
                }
                if (Input.GetKey(KeyCode.D))
                {
                    TurnRight();
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    CancelInvoke("DecelerateCar");
                    deceleratingCar = false;
                    Handbrake();
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    RecoverTraction();
                }
                if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
                {
                    ThrottleOff();
                }
                if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
                {
                    InvokeRepeating("DecelerateCar", 0f, 0.1f);
                    deceleratingCar = true;
                }
                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
                {
                    ResetSteeringAngle();
                }

            }


            // 차량 휠 메쉬의 움직임 표현
            AnimateWheelMeshes();

        }

        // 차량 속도 UI 
        public void CarSpeedUI()
        {

            if (useUI)
            {
                try
                {
                    float absoluteCarSpeed = Mathf.Abs(carSpeed);
                    carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }

        }

        // 자동차 소리를 속도에 따라서 제어, 느리면 낮은 피치 빠르면 높은 피치 
        // 초기 값 + 속도 / 100f 
        // 드리프트 소리
        public void CarSounds()
        {

            if (useSounds)
            {
                try
                {
                    if (carEngineSound != null)
                    {
                        float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
                        carEngineSound.pitch = engineSoundPitch;
                    }
                    if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                    {
                        if (!tireScreechSound.isPlaying)
                        {
                            tireScreechSound.Play();
                        }
                    }
                    else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                    {
                        tireScreechSound.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
            else if (!useSounds)
            {
                if (carEngineSound != null && carEngineSound.isPlaying)
                {
                    carEngineSound.Stop();
                }
                if (tireScreechSound != null && tireScreechSound.isPlaying)
                {
                    tireScreechSound.Stop();
                }
            }

        }

        //
        //STEERING METHODS
        //

        // steeringSpeed에 기반한 속도로 움직임 변화
        public void TurnLeft()
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
            if (steeringAxis < -1f)
            {
                steeringAxis = -1f;
            }
            var steeringAngle = steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        public void TurnRight()
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
            if (steeringAxis > 1f)
            {
                steeringAxis = 1f;
            }
            var steeringAngle = steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        // 손 때고 기본값으로 돌아옴
        public void ResetSteeringAngle()
        {
            if (steeringAxis < 0f)
            {
                steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
            }
            else if (steeringAxis > 0f)
            {
                steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
            }
            if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
            {
                steeringAxis = 0f;
            }
            var steeringAngle = steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        // 차량 바퀴 메쉬를 굴러가게 함
        void AnimateWheelMeshes()
        {
            try
            {
                Quaternion FLWRotation;
                Vector3 FLWPosition;
                frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
                frontLeftMesh.transform.position = FLWPosition;
                frontLeftMesh.transform.rotation = FLWRotation;

                Quaternion FRWRotation;
                Vector3 FRWPosition;
                frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
                frontRightMesh.transform.position = FRWPosition;
                frontRightMesh.transform.rotation = FRWRotation;

                Quaternion RLWRotation;
                Vector3 RLWPosition;
                rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
                rearLeftMesh.transform.position = RLWPosition;
                rearLeftMesh.transform.rotation = RLWRotation;

                Quaternion RRWRotation;
                Vector3 RRWPosition;
                rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
                rearRightMesh.transform.position = RRWPosition;
                rearRightMesh.transform.rotation = RRWRotation;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        //
        //ENGINE AND BRAKING METHODS
        //

        // 전진을 위해 바퀴에 토크 전달
        public void GoForward()
        {
            // 차량의 x축에 힘이 2.5f 이상 가해지면 차량은 트랙션을 잃었다는 뜻이고 그러면 파티클로 연기 시스템이 나오기 시작함
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }
            // 스로틀 파워를 1까지 설정
            throttleAxis = throttleAxis + (Time.deltaTime * 3f);
            if (throttleAxis > 1f)
            {
                throttleAxis = 1f;
            }
            // 자동차가 뒤로 가고 있다면 전진키가 브레이크를 작동시킴
            // 'z' 축의 로컬 속도가 -1f보다 작다면, 앞으로 가기 위해 양의 토크를 안전하게 적용함
            if (localVelocityZ < -1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.RoundToInt(carSpeed) < maxSpeed)
                {
                    // 최고속도에 도달하기 전에는 양의 토크가 모든 바퀴에 적용됨
                    frontLeftCollider.brakeTorque = 0;
                    frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                    frontRightCollider.brakeTorque = 0;
                    frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                    rearLeftCollider.brakeTorque = 0;
                    rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                    rearRightCollider.brakeTorque = 0;
                    rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                }
                else
                {
                    // 최대 속도(maxSpeed)가 도달되었다면, 바퀴에 토크를 적용하는 것을 멈춤
                    // 중요: maxSpeed 변수는 근사값임
                    // 자동차의 속도가 예상보다 약간 더 높을 수 있음
                    frontLeftCollider.motorTorque = 0;
                    frontRightCollider.motorTorque = 0;
                    rearLeftCollider.motorTorque = 0;
                    rearRightCollider.motorTorque = 0;
                }
            }
        }

        // 후진을 위해 바퀴에 토크를 반대로 전달
        public void GoReverse()
        {
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }

            throttleAxis = throttleAxis - (Time.deltaTime * 3f);
            if (throttleAxis < -1f)
            {
                throttleAxis = -1f;
            }
            if (localVelocityZ > 1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
                {
                    frontLeftCollider.brakeTorque = 0;
                    frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                    frontRightCollider.brakeTorque = 0;
                    frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                    rearLeftCollider.brakeTorque = 0;
                    rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                    rearRightCollider.brakeTorque = 0;
                    rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                }
                else
                {
                    frontLeftCollider.motorTorque = 0;
                    frontRightCollider.motorTorque = 0;
                    rearLeftCollider.motorTorque = 0;
                    rearRightCollider.motorTorque = 0;
                }
            }
        }

        // 전진키 후진키 모두 안누르면 토크가 0으로 감
        public void ThrottleOff()
        {
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
        }

        // decelerationMultiplier 변수에 따라 자동차을 감속시킨다. 1 - 10까지 정도임
        // 전진 후진 핸드브레이크를 누르지 않으면 0.1초마다 호출 됨
        public void DecelerateCar()
        {
            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                isDrifting = true;
                DriftCarPS();
            }
            else
            {
                isDrifting = false;
                DriftCarPS();
            }
            // 스로틀 파워를 부드럽게 0으로 만듬
            if (throttleAxis != 0f)
            {
                if (throttleAxis > 0f)
                {
                    throttleAxis = throttleAxis - (Time.deltaTime * 10f);
                }
                else if (throttleAxis < 0f)
                {
                    throttleAxis = throttleAxis + (Time.deltaTime * 10f);
                }
                if (Mathf.Abs(throttleAxis) < 0.15f)
                {
                    throttleAxis = 0f;
                }
            }
            carRigidbody.linearVelocity = carRigidbody.linearVelocity * (1f / (1f + (0.025f * decelerationMultiplier)));
            // 차량 감속을 위해 바퀴에 토크를 제거함
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
            // 차량 속도가 0.25f 미만인 매우 느린 속도라면 차를 멈추고 메소드 호출을 취소시킴
            if (carRigidbody.linearVelocity.magnitude < 0.25f)
            {
                carRigidbody.linearVelocity = Vector3.zero;
                CancelInvoke("DecelerateCar");
            }
        }

        // 사용자가 입력한 브레이크 힘에 따라 브레이크 토크입력
        public void Brakes()
        {
            frontLeftCollider.brakeTorque = brakeForce;
            frontRightCollider.brakeTorque = brakeForce;
            rearLeftCollider.brakeTorque = brakeForce;
            rearRightCollider.brakeTorque = brakeForce;
        }

        // 자동차가 미끄러짐을 일으키게 하여 드리프트를 시작
        // handbrakeDriftMultiplier 변수에 따라 미끄러짐의 정도가 결정
        // 이 값이 작으면 자동차가 많이 미끄러지지 않지만,
        // 값이 높으면 자동차가 얼음 위를 달리는 것처럼 느껴짐
        public void Handbrake()
        {
            CancelInvoke("RecoverTraction");
            // 부드럽게 미끄러지기 시작함, driftingAxis 변수가 사용됨
            // 0 - 1 사이 값임 Time.deltaTime을 사용해 부드럽게 증가함
            driftingAxis = driftingAxis + (Time.deltaTime);
            float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

            if (secureStartingPoint < FLWextremumSlip)
            {
                driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
            }
            if (driftingAxis > 1f)
            {
                driftingAxis = 1f;
            }

            if (Mathf.Abs(localVelocityX) > 2.5f)
            {
                isDrifting = true;
            }
            else
            {
                isDrifting = false;
            }
            //driftingAxis 값이 1이 아니면 바퀴가 최대 드리프트 값에 도달 하지 못한것
            // 1이 될떄까지 마찰이 계속 증가함
            if (driftingAxis < 1f)
            {
                FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                frontLeftCollider.sidewaysFriction = FLwheelFriction;

                FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                frontRightCollider.sidewaysFriction = FRwheelFriction;

                RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                rearLeftCollider.sidewaysFriction = RLwheelFriction;

                RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                rearRightCollider.sidewaysFriction = RRwheelFriction;
            }

            // 핸드브레이크 사용시 바퀴가 잠김
            // 타이어 스키드 마크 효과를 사용함
            isTractionLocked = true;
            DriftCarPS();

        }

        // isDrifting과 isTractionLocked 변수를 사용해 타이어 스키드 마크와 연기를 방출함
        public void DriftCarPS()
        {

            if (useEffects)
            {
                try
                {
                    if (isDrifting)
                    {
                        RLWParticleSystem.Play();
                        RRWParticleSystem.Play();
                    }
                    else if (!isDrifting)
                    {
                        RLWParticleSystem.Stop();
                        RRWParticleSystem.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }

                try
                {
                    if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
                    {
                        RLWTireSkid.emitting = true;
                        RRWTireSkid.emitting = true;
                    }
                    else
                    {
                        RLWTireSkid.emitting = false;
                        RRWTireSkid.emitting = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
            else if (!useEffects)
            {
                if (RLWParticleSystem != null)
                {
                    RLWParticleSystem.Stop();
                }
                if (RRWParticleSystem != null)
                {
                    RRWParticleSystem.Stop();
                }
                if (RLWTireSkid != null)
                {
                    RLWTireSkid.emitting = false;
                }
                if (RRWTireSkid != null)
                {
                    RRWTireSkid.emitting = false;
                }
            }

        }

        // 핸드브레이크 사용 중단시 트랙션 회복함
        public void RecoverTraction()
        {
            isTractionLocked = false;
            driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
            if (driftingAxis < 0f)
            {
                driftingAxis = 0f;
            }

            // driftingAxis 변수가 0이 아니면 아직 트랙션 회복을 못한 것이고 시작 차량 그립을 회복할때까지
            // 측면 마찰이 계속해서 줄어들 것임
            if (FLwheelFriction.extremumSlip > FLWextremumSlip)
            {
                FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                frontLeftCollider.sidewaysFriction = FLwheelFriction;

                FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                frontRightCollider.sidewaysFriction = FRwheelFriction;

                RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                rearLeftCollider.sidewaysFriction = RLwheelFriction;

                RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
                rearRightCollider.sidewaysFriction = RRwheelFriction;

                Invoke("RecoverTraction", Time.deltaTime);

            }
            else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
            {
                FLwheelFriction.extremumSlip = FLWextremumSlip;
                frontLeftCollider.sidewaysFriction = FLwheelFriction;

                FRwheelFriction.extremumSlip = FRWextremumSlip;
                frontRightCollider.sidewaysFriction = FRwheelFriction;

                RLwheelFriction.extremumSlip = RLWextremumSlip;
                rearLeftCollider.sidewaysFriction = RLwheelFriction;

                RRwheelFriction.extremumSlip = RRWextremumSlip;
                rearRightCollider.sidewaysFriction = RRwheelFriction;

                driftingAxis = 0f;
            }
        }

    }
}
