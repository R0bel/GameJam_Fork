﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

[Serializable]
public struct ARLevel
{
    public string name;
    public GameObject levelObj;
}

/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour, IManagedBehaviour
{
    GameManager gameManager;

    [SerializeField]
    private ARLevel[] levels = new ARLevel[2];
    private ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    public void OnStart(GameManager _manager)
    {
        gameManager = _manager;
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    public ARLevel[] ARLevels
    {
        get
        {
            return levels;
        }
    }

    void UpdateTransform(ARTrackedImage trackedImage)
    {
        foreach (ARLevel level in levels)
        {
            if (trackedImage.referenceImage.name == level.name)
            {
                // set all level inactive
                Array.ForEach(levels, l => l.levelObj.SetActive(false));

                // set level active
                level.levelObj.transform.position = trackedImage.transform.position;
                level.levelObj.transform.rotation = trackedImage.transform.rotation;
                if (!level.levelObj.activeSelf) level.levelObj.SetActive(true);
                break;
            }
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        gameManager.Events.OnTrackedImagesChanged(eventArgs);
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
    }
}
