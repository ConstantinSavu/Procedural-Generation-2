using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 300f;
    [SerializeField]
    private Transform playerBody;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private Character character;
    [SerializeField]
    private bool invertedMouse = false;

    [SerializeField]
    private bool inWater = false;

    [SerializeField]
    public CinemachineVirtualCamera camera_VM;

    [SerializeField]
    private CinemachineVolumeSettings volumeSettings;

    [SerializeField]
    public VolumeProfile solidProfile;
    public Color solidFogColor = Color.gray;
    public float solidFogEnd = 200f;

    public VolumeProfile waterProfile;
    public Color waterFogColor = Color.blue;
    public float waterFogEnd = 100f;

    float verticalRotation = 0f;

    private void Awake(){
        playerInput = GetComponentInParent<PlayerInput>();
        character = GetComponentInParent<Character>();
    }

    private void Start(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetCamera(CinemachineVirtualCamera _camera_VM){
        camera_VM = _camera_VM;
        camera_VM.m_Lens.ModeOverride = Cinemachine.LensSettings.OverrideModes.Perspective;
        camera_VM.Follow = transform;

        volumeSettings = camera_VM.GetComponent<CinemachineVolumeSettings>();
        volumeSettings.m_Profile = solidProfile;
    }

    private void Update(){

        CheckIfInWater();

        if(camera_VM != null && inWater){
            if(!volumeSettings.m_Profile.Equals(waterProfile)){
                SetWaterSettings();
            }
        }
        else if(camera_VM != null){
            if(!volumeSettings.m_Profile.Equals(solidProfile)){
                SetSolidSettings();
            }
        }

        float mouseX = playerInput.MousePosition.x * sensitivity * Time.deltaTime;
        float mouseY = playerInput.MousePosition.y * sensitivity * Time.deltaTime;

        if(invertedMouse){
            mouseY *= -1;
        }

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    void CheckIfInWater()
    {

        VoxelType voxelType = WorldDataHelper.GetVoxelFromWorldCoorinates(character.world, Vector3Int.RoundToInt(transform.position));

        if(voxelType == VoxelType.Water){
            
            inWater = true;
            
        }
        else{
            
            inWater = false;
        }

    }

    void SetWaterSettings(){
        volumeSettings.m_Profile = waterProfile;
        RenderSettings.fogColor = waterFogColor;
        RenderSettings.fogEndDistance = waterFogEnd;
    }

    void SetSolidSettings(){
        volumeSettings.m_Profile = solidProfile;
        RenderSettings.fogColor = solidFogColor;
        RenderSettings.fogEndDistance = solidFogEnd;

    }

}
