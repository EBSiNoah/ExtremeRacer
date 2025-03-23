using System;
using UnityEngine;
using UnityEngine.UI;

namespace songkim
{
    public class SoundController : MonoBehaviour
    {
        public CarController ctrl;

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

        private void Start()
        {
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
                        float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(ctrl.carRigidbody.linearVelocity.magnitude) / 25f);
                        carEngineSound.pitch = engineSoundPitch;
                    }
                    if ((ctrl.isDrifting) || (ctrl.isTractionLocked && Mathf.Abs(ctrl.carSpeed) > 12f))
                    {
                        if (!tireScreechSound.isPlaying)
                        {
                            tireScreechSound.Play();
                        }
                    }
                    else if ((!ctrl.isDrifting) && (!ctrl.isTractionLocked || Mathf.Abs(ctrl.carSpeed) < 12f))
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


        // 차량 속도 UI 
        public void CarSpeedUI()
        {

            if (useUI)
            {
                try
                {
                    float absoluteCarSpeed = Mathf.Abs(ctrl.carSpeed);
                    carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }

        }
    }
}
