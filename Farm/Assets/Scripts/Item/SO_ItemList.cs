using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_ItemList", menuName = "SO/Item/ItemList")]
public class SO_ItemList : ScriptableObject
{

    [SerializeField] public List<ItemDetails> itemDetails;

}
