using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static List<Player> Players = new List<Player>();
    public static Player LocalPlayer { get; set; }

    public GameObject playerPrefab;
    public Transform characterTransform;
    public Transform playerPanelsTransform;
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

    private void ResetValues() {
        Players = new List<Player>();
        LocalPlayer = null;
        materialIndicesShuffled = false;
    }

    private void Start() {
        ResetValues();
        if (instance == null) instance = this;
        else {
            Destroy(this);
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
            new Vector3(0, 0, -24f),
        };

        PlayerPositions[1] = new Vector3[] {
            new Vector3(0, 0, -24f),
            new Vector3(0, 0, 24f)
        };

        PlayerPositions[2] = new Vector3[] {
            new Vector3(0, 0, -24f),
            new Vector3(-24f, 0, 0),
            new Vector3(0, 0, 24f)
        };

        PlayerPositions[3] = new Vector3[] {
            new Vector3(0, 0, -24f),
            new Vector3(-24f, 0, 0),
            new Vector3(0, 0, 24f),
            new Vector3(24f, 0, 0)
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
            Vector3.up * 90,
            Vector3.up * 180,
        };

        PlayerRotations[3] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 90,
            Vector3.up * 180,
            -Vector3.up * 90,
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
        if (LocalPlayer == null) return;

        int startIdx = Players.IndexOf(LocalPlayer);

        Vector3[] positions = PlayerPositions[count - 1];
        Vector3[] rotations = PlayerRotations[count - 1];
        int[] characterIndicies = ActiveCharacterObjectsIndex[count - 1];

        for (int i = 0; i < instance.playerPanelsTransform.childCount; i++)
            instance.playerPanelsTransform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < count; i++) {
            int playerIdx = (startIdx + i) % count;
            Transform characterTransform = instance.characterTransform.GetChild(characterIndicies[i]);
            int characterMaterialIndex = characterMaterialIndices[i];
            characterTransform.gameObject.SetActive(i != startIdx);
            characterTransform.GetComponentInChildren<Renderer>().material = Global.CharacterMaterials[characterMaterialIndex];

            if (i == 0) continue;

            Players[playerIdx].transform.position = positions[playerIdx];
            Players[playerIdx].transform.localEulerAngles = rotations[playerIdx];

            Vector3 startPlayer = positions[startIdx];
            Vector3 nowPlayer = positions[playerIdx];
            if (startPlayer.x != nowPlayer.x && startPlayer.z != nowPlayer.z) {
                float mult = (i == 1) ? -1 : 1;
                Debug.Log($"Multiplier: {mult}");
                Debug.Log($"Sub: {i - startIdx}");
                Players[playerIdx].transform.position += Players[playerIdx].transform.forward * 4f;
                Players[playerIdx].transform.position += mult * Players[playerIdx].transform.right * 10f;
                Players[playerIdx].transform.eulerAngles += -mult * Vector3.up * 30f;
            }
        }

        PlayersPanelManager.SetValues(startIdx);

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

    public static int GetNumberOfUniquePlayers() {
        int count = 0;
        foreach (Player p in Players) if(p != null) count++;
        return count;
    }
}
