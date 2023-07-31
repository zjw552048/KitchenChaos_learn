using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSo/RecipeSo")]
public class RecipeSo : ScriptableObject {
    public List<KitchenObjectSo> KitchenObjectSos;
    public string recipeName;
}