using GravityPath.Enumeration;
using Microsoft.Xna.Framework;

namespace GravityPath.Contract
{
    public interface IIntersectableObject
    {
        bool Intersects(IIntersectableObject intersectableObject, ShapeObject shapeObject);
        Rectangle ObjectArea { get; }
        float Radius { get; }
        Vector2 Position { get; }
    }
}