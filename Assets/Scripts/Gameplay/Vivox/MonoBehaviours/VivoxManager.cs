using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
#if AUTH_PACKAGE_PRESENT
using Unity.Services.Authentication;
#endif

namespace Unity.MegaCity.Gameplay
{
    /// <summary>
    /// Manages Vivox service
    /// </summary>
    [RequireComponent(typeof(VivoxSession))]
    [RequireComponent(typeof(VivoxChannel))]
    [RequireComponent(typeof(VivoxDevicesVolume))]
    public class VivoxManager : MonoBehaviour
    {
        public static VivoxManager Instance { get; private set; }
        public VivoxDevicesVolume Devices { get; private set; }
        public VivoxSession Session { get; private set; }
        public VivoxChannel Channel { get; private set; }
        private bool m_IsReady;

        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Debug.LogWarning(VivoxEvents.k_MultipleVivoxComponentDetected);
                Destroy(this);
                return;
            }

            Instance = this;
            m_IsReady = false;
            Devices = GetComponent<VivoxDevicesVolume>();
            Session = GetComponent<VivoxSession>();
            Channel = GetComponent<VivoxChannel>();
        }

        private async void Start()
        {
            // if the Unity project is not linked to a Unity services project. 
            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                Debug.LogWarning(VivoxEvents.k_SetupProjectInTheCloud);
                return;
            }

            await UnityServices.InitializeAsync(new InitializationOptions());
#if AUTH_PACKAGE_PRESENT
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
#endif
            if (UnityServices.State == ServicesInitializationState.Initialized && VivoxService.Instance != null)
            {
                m_IsReady = true;
                VivoxService.Instance.Initialize();
            }
            else 
            {
                Debug.Log($"State {UnityServices.State} VivoxService Instance Is null: {VivoxService.Instance == null}");
            }
        }

        public void Logout()
        {
            // Needed to add this to prevent some unsuccessful init, we can revisit to do better
            if (m_IsReady)
            {
                Devices.SetMicrophoneMute(true);
                Session.Logout();
            }
        }

        private void OnApplicationQuit()
        {
            if (m_IsReady && Session != null)
                Session.ClosingClientConnection();
        }
    }
}
