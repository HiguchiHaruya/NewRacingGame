//using Cinemachine;
//using System.Collections;
//using System.Collections.Generic;
//using UniRx;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.PlayerLoop;

//public class CameraManager : MonoBehaviour
//{
//    [SerializeField]
//    CinemachineVirtualCamera _cam1;
//    [SerializeField]
//    CinemachineVirtualCamera _cam2;
//    private bool _cameraInput = false;
//    private void Start()
//    {
//        StartCoroutine(FirstCameraEvent());
//        InputReader.Instance.OnCameraSwitchAsObservable
//            .Where(context => context.performed) //ƒ{ƒ^ƒ“‰Ÿ‚³‚ê‚½uŠÔ‚Ì‚Ý”½‰ž‚·‚é
//            .Subscribe(_ => SwicthCamera())
//            .AddTo(this);
//    }
//    private void SwicthCamera()
//    {
//        _cameraInput = !_cameraInput;
//        if (_cameraInput)
//        {
//            ActiveCam1();
//        }
//        else if (!_cameraInput)
//        {
//            ActiveCam2();
//        }
//    }
//    IEnumerator FirstCameraEvent()
//    {
//        ActiveCam1();
//        yield return new WaitForSeconds(0.5f);
//        ActiveCam2();
//    }
//    public void ActiveCam1()
//    {
//        _cam2.Priority = 0;
//        _cam1.Priority = 50;
//    }
//    public void ActiveCam2()
//    {
//        _cam1.Priority = 0;
//        _cam2.Priority = 50;
//    }
//}
