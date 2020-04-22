using System;
using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Regions
{
    /// <summary>
    /// Regions are spacial aware areas of the map. They are used for checking reachability, and finding
    /// nearest objects / items / entities etc.
    /// Regions are defined a max size in RegionManager.cs and will always be built to follow a grid like
    /// layout.
    /// </summary>
    public class Region
    {
        public HashSet<Tile> Tiles { get; private set; }

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

        private List<EdgeSpan> spans = new List<EdgeSpan>();

        public void Add(Tile _tile)
        {
            if (Tiles == null) Tiles = new HashSet<Tile>();

            if (Tiles.Contains(_tile))
            {
                return;
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

            // Sort because the regions are created from a floodfill which means the bridges wont
            // correctly organised when detecting edge spans.
            BoundaryMap[0].Sort((_tile, _tile1) => _tile.Y.CompareTo(_tile1.Y));
            BoundaryMap[1].Sort((_tile, _tile1) => _tile.Y.CompareTo(_tile1.Y));
            BoundaryMap[2].Sort((_tile, _tile1) => _tile.X.CompareTo(_tile1.X));
            BoundaryMap[3].Sort((_tile, _tile1) => _tile.X.CompareTo(_tile1.X));

            GenerateEdgeSpans();
        }

        private void GenerateEdgeSpans()
        {
            spans.Clear();

            foreach (var link in Links)
            {
                link.Unassign(this);
            }

            Links.Clear();

            foreach (var pair in BoundaryMap)
            {
                var edgeSpan = new List<Tile>();
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
                        if (Math.Abs(tile.Y - lastSpanTile.Y) > 1)
                        {
                            spans.Add(new EdgeSpan(edgeSpan[0], EdgeSpanDirection.Right, edgeSpan.Count));
                            edgeSpan = new List<Tile>();
                            edgeSpan.Add(tile);
                            continue;
                        }
                    }
                    // Up links.
                    else
                    {
                        if (Math.Abs(tile.X - lastSpanTile.X) > 1)
                        {
                            spans.Add(new EdgeSpan(edgeSpan[0], EdgeSpanDirection.Up, edgeSpan.Count));
                            edgeSpan = new List<Tile>();
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