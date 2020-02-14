using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Microsoft.MixedReality.Toolkit.UI;

public class MapPlotter : MonoBehaviour
{
    [Tooltip("Latitude Longitude")]
    public string location;
    [Tooltip("mapZoomLevel")]
    public float zoom;
    [Tooltip("changes size scale of plot")]
    public float plotScale = 3;
    // Object which will contain instantiated prefabs in hiearchy
    [Tooltip("Object which will contain instantiated prefabs in hiearchy")]
    public GameObject MapHolder;
    // Object which will contain text in hiearchy
    [Tooltip("Object which will contain text in hiearchy")]
    public GameObject Text;
    [Tooltip("Title of plot")]
    public string titleName;
    [Tooltip("enabled 3D terrain default is flat")]
    public bool enable3DTerrain = false;
    // Start is called before the first frame update
    void Start()
    {
        MapHolder.AddComponent<AbstractMap>();
        MapOptions options=MapHolder.GetComponent<AbstractMap>().Options;
        options.locationOptions.latitudeLongitude = location;
        options.locationOptions.zoom = zoom;
        options.scalingOptions.unityTileSize = plotScale;
        IImageryLayer image=MapHolder.GetComponent<AbstractMap>().ImageLayer;
        image.SetLayerSource(ImagerySourceType.MapboxDark);
        IVectorDataLayer layer = MapHolder.GetComponent<AbstractMap>().VectorData;
        layer.SetLayerSource(VectorSourceType.MapboxStreetsWithBuildingIds);
        layer.AddPolygonFeatureSubLayer("buildings", "building");
        layer.FindFeatureSubLayerWithName("buildings").materialOptions.SetStyleType(StyleTypes.Light);
        if (enable3DTerrain)
        {
            ITerrainLayer terrain = MapHolder.GetComponent<AbstractMap>().Terrain;
            terrain.SetElevationType(ElevationLayerType.LowPolygonTerrain);
        }

        MapHolder.GetComponent<AbstractMap>().SetPlacementType(MapPlacementType.AtTileCenter);
        InitalizeInteraction();

        GameObject title = Instantiate(Text, new Vector3(-0.2f, 2.5f, -1.3f) * plotScale, Quaternion.identity);
        //place title so it is just above plot
        title.transform.position = title.transform.position + new Vector3(0, title.GetComponent<Renderer>().bounds.size.y / 2, 0) * plotScale;
        //add title 
        title.transform.parent = MapHolder.transform;
        title.GetComponent<TextMesh>().text = titleName;
        title.transform.name = "title";
        //scale the size of text depending on PlotScale
        title.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * plotScale;
        //place title so it is just above plot
        title.transform.position = title.transform.position + new Vector3(0, title.GetComponent<Renderer>().bounds.size.y / 2, 0);


    }

    private void InitalizeInteraction()
    {
        MapHolder.AddComponent<BoxCollider>();
        MapHolder.transform.gameObject.GetComponent<BoxCollider>().size = new Vector3(3, 2.5f, 3) * plotScale;
        MapHolder.transform.gameObject.GetComponent<BoxCollider>().center = new Vector3(0, 1.25f, 0) * plotScale;
        MapHolder.AddComponent<BoundingBox>();
        MapHolder.GetComponent<BoundingBox>().WireframeMaterial.color = Color.white;
        MapHolder.AddComponent<ManipulationHandler>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
