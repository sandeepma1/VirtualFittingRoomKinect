using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenPose.Example
{
    public class ShirtContainer2d : MonoBehaviour
    {
        public int PoseKeypointsCount = 25;
        [SerializeField] RectTransform PoseParent;
        [SerializeField] List<RectTransform> poseJoints = new List<RectTransform>();

        private void Start()
        {
            if (PoseParent)
            {
                Debug.Assert(PoseParent.childCount == PoseKeypointsCount, "Pose joint count not match");
                for (int i = 0; i < PoseKeypointsCount; i++)
                {
                    poseJoints.Add(PoseParent.GetChild(i) as RectTransform);
                }
            }
        }

        public void DrawBody(ref OPDatum datum, int bodyIndex, float scoreThres)
        {
            if (datum.poseKeypoints == null || bodyIndex >= datum.poseKeypoints.GetSize(0))
            {
                PoseParent.gameObject.SetActive(false);
                return;
            }
            else
            {
                PoseParent.gameObject.SetActive(true);
            }
            // Pose
            for (int part = 0; part < 7; part++)
            {
                // Joints overflow
                if (part >= datum.poseKeypoints.GetSize(1))
                {
                    poseJoints[part].gameObject.SetActive(false);
                    continue;
                }
                // Compare score
                if (datum.poseKeypoints.Get(bodyIndex, part, 2) <= scoreThres)
                {
                    poseJoints[part].gameObject.SetActive(false);
                }
                else
                {
                    poseJoints[part].gameObject.SetActive(true);
                    Vector3 pos = new Vector3(datum.poseKeypoints.Get(bodyIndex, part, 0), datum.poseKeypoints.Get(bodyIndex, part, 1), 0f);
                    poseJoints[part].localPosition = pos;
                }
            }
        }
    }
}