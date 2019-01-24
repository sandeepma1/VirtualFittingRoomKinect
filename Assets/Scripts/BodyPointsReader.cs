using UnityEngine;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using Windows.Kinect;
using System;

public class BodyPointsReader : MonoBehaviour
{
    public static Action<bool> OnRightHandClicked;
    public static Action<bool> OnLeftHandClicked;

    [SerializeField] private HandCursor rightHandCursor;
    [SerializeField] private HandCursor leftHandCursor;
    [SerializeField] private float neckToHipDistance;
    [SerializeField] private UiItemSelector shirtUiSelector;
    [SerializeField] private UiItemSelector pantUiSelector;
    private Dictionary<ulong, GameObject> bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager bodyManager;
    private Dictionary<JointType, JointType> boneMap = new Dictionary<JointType, JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    public CoordinateMapper coordinateMapper;

    private Vector2 lShoulder, rShoulder, neck;
    private Vector2 lHip, rHip, spineBase;

    private void Start()
    {
        bodyManager = GetComponent<BodySourceManager>();
        if (bodyManager._Sensor == null)
        {
            bodyManager._Sensor = KinectSensor.GetDefault();
        }

        coordinateMapper = bodyManager._Sensor.CoordinateMapper;
    }

    private void Update()
    {
        if (bodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = bodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(bodies[trackingId]);
                bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!bodies.ContainsKey(body.TrackingId))
                {
                    bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                RefreshBodyObject(body, bodies[body.TrackingId]);
            }
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            //GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject jointObj = new GameObject();
            jointObj.transform.localScale = new Vector3(30f, 30f, 30f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        CameraSpacePoint[] cameraSpacePoints = new CameraSpacePoint[body.Joints.Count];
        int counter = 0;
        foreach (KeyValuePair<JointType, Kinect.Joint> joint in body.Joints)
        {
            cameraSpacePoints[counter] = (joint.Value.Position);
            ++counter;
        }
        ColorSpacePoint[] colorPoints = new ColorSpacePoint[cameraSpacePoints.Length];
        coordinateMapper.MapCameraPointsToColorSpace(cameraSpacePoints, colorPoints);

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            if (boneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[boneMap[jt]];
            }
            Vector3 position = new Vector3(colorPoints[(int)jt].X, -colorPoints[(int)jt].Y, 0);
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            if (float.IsNaN(position.x) || float.IsInfinity(position.x) ||
                float.IsNaN(position.y) || float.IsInfinity(position.y))
            {
                continue;
            }
            jointObj.localPosition = position;
            CalculateOutfitItems(jt, position);
            if (jt == JointType.HandRight)
            {
                if (body.HandRightConfidence == TrackingConfidence.High && body.HandRightState == HandState.Closed)
                {
                    rightHandCursor.IsClicked(position);
                    OnRightHandClicked?.Invoke(true);
                }
                else
                {
                    rightHandCursor.IsNotClicked(position);
                    OnRightHandClicked?.Invoke(false);
                }
            }

            if (jt == JointType.HandLeft)
            {
                if (body.HandLeftConfidence == TrackingConfidence.High && body.HandLeftState == HandState.Closed)
                {
                    leftHandCursor.IsClicked(position);
                    OnLeftHandClicked?.Invoke(true);
                }
                else
                {
                    leftHandCursor.IsNotClicked(position);
                    OnLeftHandClicked?.Invoke(false);
                }
            }

        }
    }

    private void CalculateOutfitItems(JointType jointType, Vector3 position)
    {
        //calculate Shirt
        if (jointType == JointType.Neck)
        {
            neck = position;
            shirtUiSelector.MoveItem(position);
        }
        else if (jointType == JointType.ShoulderLeft)
        {
            lShoulder = position;
        }
        else if (jointType == JointType.ShoulderRight)
        {
            rShoulder = position;
        }
        //calculate pant
        else if (jointType == JointType.SpineBase)
        {
            spineBase = position;
            pantUiSelector.MoveItem(position);
        }
        else if (jointType == JointType.HipLeft)
        {
            lHip = position;
        }
        else if (jointType == JointType.HipRight)
        {
            rHip = position;
        }
        neckToHipDistance = Vector3.Distance(neck, spineBase);
        shirtUiSelector.ScaleItem(Vector3.Distance(rShoulder, lShoulder));
        pantUiSelector.ScaleItem(Vector3.Distance(rHip, lHip));
    }
}
