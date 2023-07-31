using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour {
    private PlatesCounter platesCounter;

    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private Transform plateSpawnParent;

    private List<Transform> plates;
    private const float PLATE_OFFSET_Y = 0.1f;

    private void Awake() {
        platesCounter = GetComponent<PlatesCounter>();
        plates = new List<Transform>();
    }

    private void Start() {
        platesCounter.SpawnPlateAction += OnSpawnPlateAction;
        platesCounter.RemovePlateAction += OnRemovePlateAction;
    }

    private void OnRemovePlateAction() {
        var lastPlateIndex = plates.Count - 1;
        var lastPlate = plates[lastPlateIndex];
        plates.RemoveAt(lastPlateIndex);

        Destroy(lastPlate.gameObject);
    }

    private void OnSpawnPlateAction() {
        var plate = Instantiate(plateVisualPrefab, plateSpawnParent);
        plate.localPosition = new Vector3(0, PLATE_OFFSET_Y * plates.Count, 0);
        plates.Add(plate);
    }
}