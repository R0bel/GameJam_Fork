using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour
{
    public GameObject level1;
    public GameObject level2;
    ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void Start()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateTransform(ARTrackedImage trackedImage)
    {
        switch (trackedImage.referenceImage.name)
        {
            // tracked image for level1
            case "FurtwangenBack2":
                level1.transform.position = trackedImage.transform.position;
                level1.transform.rotation = trackedImage.transform.rotation;
                if (!level1.gameObject.activeSelf)
                {
                    level2.gameObject.SetActive(false);
                    level1.gameObject.SetActive(true);
                }
                break;
            // tracked image for level2
            case "FurtwangenFront1":
                level2.transform.position = trackedImage.transform.position;
                level2.transform.rotation = trackedImage.transform.rotation;
                if (!level2.gameObject.activeSelf)
                {
                    level1.gameObject.SetActive(false);
                    level2.gameObject.SetActive(true);
                }
                break;
        }

    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTransform(trackedImage);
            Info(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTransform(trackedImage);
            Info(trackedImage);
        }
    }

    void Info(ARTrackedImage trackedImage)
    {
        Debug.Log("trackedImage: " + trackedImage.transform);
        Debug.Log("referenceImage: " + trackedImage.referenceImage.name);
        Debug.Log("levelObj position: " + level1.transform.position);
    }
}
