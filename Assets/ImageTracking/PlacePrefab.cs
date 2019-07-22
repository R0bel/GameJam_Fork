using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlacePrefab : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabParent;
    [SerializeField]
    private bool prefabIsSet;

    private ARTrackedImageManager arTrackedImgManager;

    private void Awake()
    {
        arTrackedImgManager = GetComponent<ARTrackedImageManager>();
        prefabIsSet = false;
    }

    private void OnEnable()
    {
        arTrackedImgManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        arTrackedImgManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (!prefabIsSet)
        {
            foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
                SetPrefab(trackedImage);
                prefabIsSet = true;
            }

        }
    }

    private void SetPrefab(ARTrackedImage _trackedImage)
    {
        prefabParent.transform.position = _trackedImage.transform.GetChild(0).gameObject.transform.position;
        Debug.Log(prefabParent.transform.position);
        GameObject prefab = Resources.Load<GameObject>("Cube");
        prefab.transform.parent = prefabParent.transform;
        Debug.Log(prefab.transform.parent);

    }
}
