using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using Mathd;

public class Trigger : MonoBehaviour, IMixedRealityHandJointHandler
{
    [HideInInspector]
    public string tagName=null;
    public string handName = null;
    [HideInInspector]
    public Vector3d closePos = Vector3.zero;
    ObjectManipulator manipulator;
    Handedness hand;
    public bool isManipulating;
    void Start()
    {
        hand = Handedness.Right; // Set the hand to track
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }
    private void OnDestroy()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }
    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        if (eventData.Handedness == hand)
        {
            if (eventData.InputData.TryGetValue(TrackedHandJoint.IndexTip, out MixedRealityPose pose))
            {
                Collider[] colliders = Physics.OverlapSphere(pose.Position, 0.01f); // Use a small radius to avoid detecting unwanted objects
                foreach (Collider collider in colliders)
                {
                    GameObject targetObject = collider.gameObject;
                    if (targetObject != null)
                    {
                        manipulator = targetObject.GetComponent<ObjectManipulator>();
                        if (manipulator != null)
                        {
                            closePos = manipulator.HostTransform.position;
                            manipulator.OnManipulationStarted.AddListener(OnManipulationStarted);
                            manipulator.OnManipulationEnded.AddListener(OnManipulationEnded);
                            break; // Only add listener to the first object
                        }
                    }
                }
            }
        }
    }

    void OnManipulationStarted(ManipulationEventData eventData)
    {
        if (eventData.ManipulationSource != null)
        {
            isManipulating = true;
            tagName = eventData.ManipulationSource.gameObject.transform.parent.name;
            handName = eventData.ManipulationSource.name;
            closePos = eventData.Pointer.Position;
        }
    }
    void OnManipulationEnded(ManipulationEventData eventData)
    {
        isManipulating = false;
        if (manipulator != null)
        {
            closePos = manipulator.HostTransform.position;
            tagName = null;
            handName = null;
        }
        manipulator = null;
    }
}
