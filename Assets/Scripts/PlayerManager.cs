using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static List<Player> Players = new List<Player>();
    public static Player LocalPlayer { get; set; }

    public GameObject playerPrefab;
    public Transform characterTransform;
    public Transform pivot;
    public float boundSlider = 0.05f;

    public static Vector3[][] PlayerPositions = new Vector3[4][];
    public static Vector3[][] PlayerRotations = new Vector3[4][];
    public static Vector3[][] PivotRotations = new Vector3[4][];
    public static int[][] ActiveCharacterObjectsIndex = new int[4][];

    private static Camera MainCamera;

    public static PlayerManager instance;

    public static List<int> characterMaterialIndices = new List<int>(4) {
        0,1,2,3
    };
    public static bool materialIndicesShuffled = false;

    private void Start() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        MainCamera = GameManager.MainCamera;
        InitializePositions();
    }

    private void InitializePositions() {
        float distanceCameraToOrigin = Vector3.Distance(MainCamera.transform.position, Vector3.zero);
        float halfWidth = Screen.width / 2;
        float halfHeight = Screen.height / 2;
        PlayerPositions[0] = new Vector3[] {
            Vector3.zero
        };

        PlayerPositions[1] = new Vector3[] {
            Vector3.zero,
            new Vector3(0, 0, 24f)
        };

        PlayerPositions[2] = new Vector3[] {
            Vector3.zero,
            new Vector3(-20f, 0, 10f),
            new Vector3(0, 0, 24f)
        };

        PlayerPositions[3] = new Vector3[] {
            Vector3.zero,
            new Vector3(-20f, 0, 10f),
            new Vector3(0, 0, 24f),
            new Vector3(20f, 0, 10f)
        };

        PlayerRotations[0] = new Vector3[] {
            Vector3.zero,
        };

        PlayerRotations[1] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 180,
        };

        PlayerRotations[2] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 120,
            Vector3.up * 180,
        };

        PlayerRotations[3] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 120,
            Vector3.up * 180,
            -Vector3.up * 120,
        };

        PivotRotations[0] = new Vector3[] {
            Vector3.zero
        };

        PivotRotations[1] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 180f
        };

        PivotRotations[2] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 90f,
            Vector3.up * 180f
        };

        PivotRotations[3] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 90f,
            Vector3.up * 180f,
            Vector3.up * 270f
        };

        ActiveCharacterObjectsIndex[0] = new int[] { 0 };
        ActiveCharacterObjectsIndex[1] = new int[] { 0, 2 };
        ActiveCharacterObjectsIndex[2] = new int[] { 0, 1, 2 };
        ActiveCharacterObjectsIndex[3] = new int[] { 0, 1, 2, 3 };
    }

    public static void AssignPositions() {
        int count = Players.Count;
        if (count == 0) return;

        int startIdx = 0;
        for (int i = 0; i < count; i++) {
            if (!Players[i].PV.IsMine) continue;
            startIdx = i;
            break;
        }

        Vector3[] positions = PlayerPositions[count - 1];
        Vector3[] rotations = PlayerRotations[count - 1];
        int[] characterIndicies = ActiveCharacterObjectsIndex[count - 1];

        for (int i = 0; i < count; i++) {
            Transform characterTransform = instance.characterTransform.GetChild(characterIndicies[i]);
            int characterMaterialIndex = characterMaterialIndices[i];
            characterTransform.gameObject.SetActive(i != startIdx);
            characterTransform.GetComponentInChildren<Renderer>().material = Global.CharacterMaterials[characterMaterialIndex];

            if (i == 0) continue;
            int playerIdx = (startIdx + i) % count;
            Players[playerIdx].transform.localPosition = positions[i];
            Players[playerIdx].transform.localEulerAngles = rotations[i];
        }

        for (int i = 0; i < count; i++) {
            int playerIdx = (startIdx + i) % count;
        }

        instance.pivot.eulerAngles = PivotRotations[count - 1][startIdx];
    }

    public static void DisableCharacters() {
        foreach (Transform character in instance.characterTransform) {
            character.gameObject.SetActive(false);
        }
    }

    public static void SpawnPlayer() {
        Player player = PhotonNetwork.Instantiate("Prefabs/" + instance.playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        Transform cam = MainCamera.transform;

        if (player.PV.IsMine) {
            Vector3 viewportBottom = new Vector3(0.5f, instance.boundSlider, 10f);
            Vector3 worldPosition = Camera.main.ViewportToWorldPoint(viewportBottom);

            player.transform.position = worldPosition;
            player.transform.LookAt(cam.position, Vector3.up);
            player.transform.eulerAngles = Vector3.right * player.transform.eulerAngles.x;
        }
    }
}
