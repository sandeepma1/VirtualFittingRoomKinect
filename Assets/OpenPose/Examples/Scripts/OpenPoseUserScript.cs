using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example
{
    public class OpenPoseUserScript : MonoBehaviour
    {
        // The 2D human to control
        [SerializeField] HumanController2D humanController2D;
        [SerializeField] ShirtContainer2d shirtContainer2d;
        [SerializeField] ImageRenderer imageRenderer;
        [SerializeField] Text fpsText;
        [SerializeField] Text peopleText;
        [SerializeField] private bool showPeopleCount;
        [SerializeField] private bool showFPS;
        [SerializeField] private bool renderBgImg = true;
        [SerializeField] [Range(0f, 1f)] private float frameRateSmoothRatio = 0.8f;
        // User settings
        [SerializeField] private ProducerType inputType = ProducerType.Webcam;
        [SerializeField] private string producerString = "-1";
        [SerializeField] private int maxPeople = 1;
        [SerializeField] private float renderThreshold = 0.05f;
        [SerializeField] private bool handEnabled = false;
        [SerializeField] private bool faceEnabled = false;
        [SerializeField] private Vector2Int netResolution = new Vector2Int(-1, 368);
        [SerializeField] private Vector2Int handResolution = new Vector2Int(368, 368);
        [SerializeField] private Vector2Int faceResolution = new Vector2Int(368, 368);

        private int numberPeople = 0;

        private float avgFrameRate = 0f;
        private float avgFrameTime = -1f;
        private float lastFrameTime = -1f;
        //private HumanController2D humanCont;
        // Output control
        private OPDatum datum;

        private void Start()
        {
            //humanCont = Instantiate(humanController2D, humanContainer);
            print("Start");
            // Register callbacks
            OPWrapper.OPRegisterCallbacks();
            // Enable OpenPose run multi-thread (default true)
            OPWrapper.OPEnableMultiThread(true);
            // Enable OpenPose log to unity (default true)
            OPWrapper.OPEnableDebug(true);
            // Enable OpenPose output to unity (default true)
            OPWrapper.OPEnableOutput(true);
            // Enable receiving image (default false)
            OPWrapper.OPEnableImageOutput(renderBgImg);
            // Configure OpenPose with default value, or using specific configuration for each
            /* OPWrapper.OPConfigureAllInDefault(); */
            UserConfigureOpenPose();

            // Start OpenPose
            OPWrapper.OPRun();
        }

        private void LateUpdate()
        {
            // New data received
            if (OPWrapper.OPGetOutput(out datum))
            {
                shirtContainer2d.DrawBody(ref datum, 0, renderThreshold);
                imageRenderer.UpdateImage(datum.cvInputData);
                // Number of people
                if (showPeopleCount)
                {
                    NumberOfPeople();
                }
                if (showFPS)
                {
                    CalculateFPS();
                }
            }
        }

        private void UserConfigureOpenPose()
        {
            OPWrapper.OPConfigurePose(
                /* enable */ true, /* netInputSize */ netResolution, /* outputSize */ null,
                /* keypointScaleMode */ ScaleMode.InputResolution,
                /* gpuNumber */ -1, /* gpuNumberStart */ 0, /* scalesNumber */ 1, /* scaleGap */ 0.3f,
                /* renderMode */ RenderMode.Gpu, /* poseModel */ PoseModel.MPI_15_4,
                /* blendOriginalFrame */ true, /* alphaKeypoint */ 0.6f, /* alphaHeatMap */ 0.7f,
                /* defaultPartToRender */ 0, /* modelFolder */ null,
                /* heatMapTypes */ HeatMapType.None, /* heatMapScaleMode */ ScaleMode.UnsignedChar,
                /* addPartCandidates */ false, /* renderThreshold */ renderThreshold, /* numberPeopleMax */ maxPeople,
                /* maximizePositives */ false, /* fpsMax fps_max */ -1.0,
                /* protoTxtPath */ "", /* caffeModelPath */ "");

            //OPWrapper.OPConfigureHand(
            //    /* enable */ handEnabled, /* netInputSize */ handResolution,
            //    /* scalesNumber */ 1, /* scaleRange */ 0.4f, /* tracking */ false,
            //    /* renderMode */ RenderMode.None,
            //    /* alphaKeypoint */ 0.6f, /* alphaHeatMap */ 0.7f, /* renderThreshold */ 0.2f);

            //OPWrapper.OPConfigureFace(
            //    /* enable */ faceEnabled, /* netInputSize */ faceResolution, /* renderMode */ RenderMode.None,
            //    /* alphaKeypoint */ 0.6f, /* alphaHeatMap */ 0.7f, /* renderThreshold */ 0.4f);

            //OPWrapper.OPConfigureExtra(
            //    /* reconstruct3d */ false, /* minViews3d */ -1, /* identification */ false, /* tracking */ -1,
            //    /* ikThreads */ 0);

            //OPWrapper.OPConfigureInput(
            //    /* producerType */ inputType, /* producerString */ producerString,
            //    /* frameFirst */ 0, /* frameStep */ 1, /* frameLast */ ulong.MaxValue,
            //    /* realTimeProcessing */ false, /* frameFlip */ false,
            //    /* frameRotate */ 0, /* framesRepeat */ false,
            //    /* cameraResolution */ null, /* cameraParameterPath */ null,
            //    /* undistortImage */ false, /* numberViews */ -1);

            //OPWrapper.OPConfigureOutput(
            //    /* verbose */ -1.0, /* writeKeypoint */ "", /* writeKeypointFormat */ DataFormat.Yml,
            //    /* writeJson */ "", /* writeCocoJson */ "", /* writeCocoFootJson */ "", /* writeCocoJsonVariant */ 1,
            //    /* writeImages */ "", /* writeImagesFormat */ "png", /* writeVideo */ "", /* writeVideoFps */ 30.0,
            //    /* writeHeatMaps */ "", /* writeHeatMapsFormat */ "png", /* writeVideo3D */ "",
            //    /* writeVideoAdam */ "", /* writeBvh */ "", /* udpHost */ "", /* udpPort */ "8051");

            //OPWrapper.OPConfigureGui(
            //    /* displayMode */ DisplayMode.NoDisplay, /* guiVerbose */ false, /* fullScreen */ false);
        }

        private void NumberOfPeople()
        {
            if (datum.poseKeypoints == null || datum.poseKeypoints.Empty())
            {
                numberPeople = 0;
            }
            else
            {
                numberPeople = datum.poseKeypoints.GetSize(0);
            }
            peopleText.text = "People: " + numberPeople;
        }

        private void CalculateFPS()
        {
            if (lastFrameTime > 0f)
            {
                if (avgFrameTime < 0f) avgFrameTime = Time.time - lastFrameTime;
                else
                {
                    avgFrameTime = Mathf.Lerp(Time.time - lastFrameTime, avgFrameTime, frameRateSmoothRatio);
                    avgFrameRate = 1f / avgFrameTime;
                }
            }
            lastFrameTime = Time.time;
            fpsText.text = avgFrameRate.ToString("F1");
        }
    }
}
