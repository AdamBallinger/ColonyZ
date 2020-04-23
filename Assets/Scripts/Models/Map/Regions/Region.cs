using System;
using System.Collections.Generic;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;

namespace Models.Map.Regions
{
    /// <summary>
    /// Regions are spacial aware areas of the map. They are used for checking reachability, and finding
    /// nearest objects / items / entities etc.
    /// Regions are limited to the size of a WorldChunk.
    /// </summary>
    public class Region
    {
        public HashSet<Tile> Tiles { get; } = new HashSet<Tile>();

        public List<RegionLink> Links { get; } = new List<RegionLink>();

        /// <summary>
        /// Map of tiles that are providing access to another region.
        /// 0 - Tiles granting access from the right.
        /// 1 - Tiles granting access upwards.
        /// </summary>
        public Dictionary<int, List<Tile>> BoundaryMap { get; private set; }

        /// <summary>
        /// Tiles along the edge of the region.
        /// </summary>
        public HashSet<Tile> EdgeTiles { get; } = new HashSet<Tile>();

        public bool IsDoor { get; private set; }

        private List<EdgeSpan> spans = new List<EdgeSpan>();
        private List<Tile> edgeSpan = new List<Tile>(12);

        public void Add(Tile _tile)
        {
            if (Tiles.Contains(_tile))
            {
                return;
            }

            if (_tile.HasObject && _tile.Object is DoorObject)
            {
                IsDoor = true;
            }

            Tiles.Add(_tile);
            _tile.Region = this;
        }

        public void CalculateBoundaryTiles()
        {
            if (BoundaryMap == null)
            {
                BoundaryMap = new Dictionary<int, List<Tile>>();
                BoundaryMap.Add(0, new List<Tile>()); // Left
                BoundaryMap.Add(1, new List<Tile>()); // Right
                BoundaryMap.Add(2, new List<Tile>()); // Up
                BoundaryMap.Add(3, new List<Tile>()); // Down
            }

            BoundaryMap[0].Clear();
            BoundaryMap[1].Clear();
            BoundaryMap[2].Clear();
            BoundaryMap[3].Clear();
            EdgeTiles.Clear();

            foreach (var tile in Tiles)
            {
                if (IsDoor)
                {
                    // Door is a up/down access.
                    if (tile.Left != null && tile.Left.HasObject)
                    {
                        BoundaryMap[2].Add(tile.Up);
                        BoundaryMap[3].Add(tile);
                    }
                    // Door is left/right access.
                    else
                    {
                        BoundaryMap[0].Add(tile);
                        BoundaryMap[1].Add(tile.Right);
                    }

                    break;
                }

                if (tile.Left?.Region == tile.Region
                    && tile.Right?.Region == tile.Region
                    && tile.Up?.Region == tile.Region
                    && tile.Down?.Region == tile.Region) continue;

                if (tile.Left?.GetEnterability() != TileEnterability.None
                    && tile.Left?.Region != tile.Region)
                {
                    BoundaryMap[0].Add(tile);
                    EdgeTiles.Add(tile);
                }

                if (tile.Right?.GetEnterability() != TileEnterability.None
                    && tile.Right?.Region != tile.Region)
                {
                    BoundaryMap[1].Add(tile.Right);
                    EdgeTiles.Add(tile);
                }

                if (tile.Up?.GetEnterability() != TileEnterability.None
                    && tile.Up?.Region != tile.Region)
                {
                    BoundaryMap[2].Add(tile.Up);
                    EdgeTiles.Add(tile);
                }

                if (tile.Down?.GetEnterability() != TileEnterability.None
                    && tile.Down?.Region != tile.Region)
                {
                    BoundaryMap[3].Add(tile);
                    EdgeTiles.Add(tile);
                }
            }

            // Sort because the regions are created from a floodfill which means the bridges wont be
            // correctly organised when detecting edge spans.
            if (!IsDoor)
            {
                BoundaryMap[0].Sort((_tile, _tile1) => _tile.Y.CompareTo(_tile1.Y));
                BoundaryMap[1].Sort((_tile, _tile1) => _tile.Y.CompareTo(_tile1.Y));
                BoundaryMap[2].Sort((_tile, _tile1) => _tile.X.CompareTo(_tile1.X));
                BoundaryMap[3].Sort((_tile, _tile1) => _tile.X.CompareTo(_tile1.X));
            }

            GenerateEdgeSpans();
        }

        private void GenerateEdgeSpans()
        {
            // TODO: This function need to be re written to properly detect edge spans that are not directly on the
            // boundary of a region.. Hard af to explain.. Its down to the sorting of the boundary map tiles..
            spans.Clear();

            foreach (var link in Links)
            {
                link.Unassign(this);
            }

            Links.Clear();

            foreach (var pair in BoundaryMap)
            {
                edgeSpan.Clear();
                var newSpan = true;
                var spanDir = pair.Key == 0 || pair.Key == 1 ? EdgeSpanDirection.Right : EdgeSpanDirection.Up;

                foreach (var tile in pair.Value)
                {
                    // A new span started so auto add this tile to the span list.
                    if (newSpan)
                    {
                        edgeSpan.Add(tile);
                        newSpan = false;
                        continue;
                    }

                    var lastSpanTile = edgeSpan[edgeSpan.Count - 1];

                    // Right links.
                    if (spanDir == EdgeSpanDirection.Right)
                    {
                        if (Math.Abs(tile.X - lastSpanTile.X) > 0)
                        {
                            // If this is true, then this boundary tile is connecting to a door region.
                            // Door regions can only be 1 tile. This is probably a bit hacky but it works.
                            spans.Add(new EdgeSpan(tile, spanDir, 1));
                            continue;
                        }

                        if (Math.Abs(tile.Y - lastSpanTile.Y) > 1)
                        {
                            spans.Add(new EdgeSpan(edgeSpan[0], spanDir, edgeSpan.Count));
                            edgeSpan.Clear();
                            edgeSpan.Add(tile);
                            continue;
                        }
                    }
                    // Up links.
                    else
                    {
                        if (Math.Abs(tile.Y - lastSpanTile.Y) > 0)
                        {
                            // Refer to above.
                            spans.Add(new EdgeSpan(tile, spanDir, 1));
                            continue;
                        }

                        if (Math.Abs(tile.X - lastSpanTile.X) > 1)
                        {
                            spans.Add(new EdgeSpan(edgeSpan[0], spanDir, edgeSpan.Count));
                            edgeSpan.Clear();
                            edgeSpan.Add(tile);
                            continue;
                        }
                    }

                    edgeSpan.Add(tile);
                }

                if (edgeSpan.Count > 0)
                {
                    spans.Add(new EdgeSpan(edgeSpan[0], spanDir, edgeSpan.Count));
                }
            }

            foreach (var span in spans)
            {
                var link = RegionLinksDatabase.LinkFromSpan(span);
                link.Assign(this);
                Links.Add(link);
            }
        }
    }
}