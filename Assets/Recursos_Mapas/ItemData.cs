using UnityEngine;

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string nombreItem;
    public Sprite icono;
    [TextArea] public string descripcion;
    public int maxStack = 1; // 1 = no apilable, 99 = pociones, etc.
    public bool esConsumible;
}