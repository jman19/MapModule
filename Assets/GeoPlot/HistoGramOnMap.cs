namespace DataVisualization.Plotter
{
    using System;
    using Mapbox.Utils;
	using Mapbox.Unity.Map;
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

    public class HistoGramOnMap : MonoBehaviour
	{
        [Tooltip("The map to place the data points on")]
        public AbstractMap map;

        [Tooltip("The location to place data points on")]
        [Geocode]
		public string[] locationStrings;
        [Tooltip("The location height of each histogram bar")]
        public List<float> heightValues;
        [Tooltip("The color of each histogram bar (default is white)")]
        public List<Color> colors;
        Vector2d[] _locations;

        [Tooltip("changes size scale of the data points")]
        public float spawnScale = 0.05f;
        [Tooltip("changes max height of the bar")]
        public float HeightScaleMax = 0.95f;
        [Tooltip("changes min height of the bar")]
        public float HeightScaleMin = 0.05f;

        List <GameObject> _spawnedObjects;
        private float maxHeight;
        private float minHeight;
		void Start()
		{
            //if colors was not set default to white
            if (colors == null)
            {
                colors = new List<Color>();
                for (var i=0; i < heightValues.Count; i++)
                {
                    colors.Add(Color.white);
                }
            }
            maxHeight = FindMaxValue(heightValues);
            minHeight = FindMinValue(heightValues);
            _locations = new Vector2d[locationStrings.Length];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < locationStrings.Length; i++)
			{
				var locationString = locationStrings[i];
				_locations[i] = Conversions.StringToLatLon(locationString);
                var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                instance.GetComponent<Renderer>().material.color = colors[i];
                instance.transform.localScale = new Vector3(spawnScale, (normalize(heightValues[i],maxHeight,minHeight)* HeightScaleMax)+ HeightScaleMin, spawnScale);
                instance.transform.parent = gameObject.transform.parent;
                instance.transform.localPosition = map.GeoToWorldPosition(_locations[i], true);
				_spawnedObjects.Add(instance);
			}
		}

		private void Update()
		{
            GameObject mapHolder = gameObject.transform.parent.gameObject;
            Vector3 currPos = mapHolder.transform.position;
            Vector3 currScale = mapHolder.transform.localScale;
            Quaternion rot = mapHolder.transform.rotation;

            mapHolder.transform.position=new Vector3(0, 0, 0);
            mapHolder.transform.localScale = new Vector3(1, 1, 1);
            mapHolder.transform.rotation = Quaternion.identity;
            int count = _spawnedObjects.Count;
			for (int i = 0; i < count; i++)
			{
				var spawnedObject = _spawnedObjects[i];
				var location = _locations[i];
                spawnedObject.transform.localPosition = map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(spawnScale, (normalize(heightValues[i], maxHeight, minHeight) * HeightScaleMax)+ HeightScaleMin, spawnScale);
                //postion the histogram bar above map
                spawnedObject.transform.localPosition = new Vector3(spawnedObject.transform.position.x, spawnedObject.transform.position.y+spawnedObject.transform.localScale.y/2, spawnedObject.transform.position.z);
            }
            mapHolder.transform.position = currPos;
            mapHolder.transform.rotation = rot;
            mapHolder.transform.localScale = currScale;
		}

        private float FindMaxValue(List<float> values)
        {
            //set initial value to first value
            float maxValue = values[0];

            //Loop through, overwrite existing maxValue if new value is larger
            for (var i = 0; i < values.Count; i++)
            {
                if (maxValue < values[i])
                    maxValue = values[i];
            }

            //Spit out the max value
            return maxValue;
        }

        private float FindMinValue(List<float> values)
        {

            float minValue = values[0];

            //Loop through, overwrite existing minValue if new value is smaller
            for (var i = 0; i < values.Count; i++)
            {
                if (values[i] < minValue)
                    minValue = values[i];
            }

            return minValue;
        }

        private float normalize(float value, float max, float min)
        {
            //if values are all zero or constant
            if (max - min == 0)
            {
                return 1;
            }
            else
            {
                return (value - min) / (max - min);
            }
        }
    }
}