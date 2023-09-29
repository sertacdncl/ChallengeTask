using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

namespace EpicToonFX
{
    public class ETFXLightFade : MonoBehaviour
    {
        [Header("Seconds to dim the light")]
        public float life = 0.2f;
        public bool killAfterLife = true;

        private UniversalAdditionalLightData liData;
        private Light li;
        private float initIntensity;

        // Use this for initialization
        void Start()
        {
			var universalAdditionalLightData = GetComponent<UniversalAdditionalLightData>();
			if (universalAdditionalLightData)
			{
				liData = universalAdditionalLightData;
			}
            if (gameObject.GetComponent<Light>())
            {
                li = gameObject.GetComponent<Light>();
                initIntensity = li.intensity;
            }
            else
                print("No light object found on " + gameObject.name);
        }

        // Update is called once per frame
        void Update()
        {
            if (gameObject.GetComponent<Light>())
            {
                li.intensity -= initIntensity * (Time.deltaTime / life);
				if (killAfterLife && li.intensity <= 0)
					//Destroy(gameObject);
				{
					Destroy(liData);
					Destroy(gameObject.GetComponent<Light>());
					
				}
            }
        }
    }
}