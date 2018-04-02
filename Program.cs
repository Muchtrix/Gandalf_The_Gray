using System;
using System.Collections.Generic;
using System.Linq;
using Game = System.Collections.Generic.HashSet<int>;

namespace Gandalf
{
    static class Program
    {
        static void Main(string[] args)
        {
            var helper = new ParityGameHelper();
            Console.WriteLine(helper.SolveGame());
        }

        public static Game Select(this (Game, Game) obj, int selector) {
            return (selector == 0) ? obj.Item1 : obj.Item2;
        }
    }

    class ParityGameHelper {
        (int Color, int Player)[] Nodes;
        List<int>[] Edges;

        private static Game EmptyGame {get;} = new Game();

        private static int[] ParseLine() => Console.ReadLine().Split(' ').Select(x => int.Parse(x)).ToArray();

        public int SolveGame() {
            var parameters = ParseLine();
            Nodes = new (int, int)[parameters[0] + parameters[1]];
            Edges = new List<int>[parameters[0] + parameters[1]];
            for (int i = 0; i < Edges.Length; ++i) Edges[i] = new List<int>();
            for(int i = 0; i < parameters[0]; ++i) {
                var line = ParseLine();
                Nodes[line[0]] = (line[1], 0);
            }
            for(int i = 0; i < parameters[1]; ++i) {
                var line = ParseLine();
                Nodes[line[0]] = (line[1], 1);
            }
            for(int i = 0; i < parameters[2]; ++i) {
                var line = ParseLine();
                Edges[line[0]].Add(line[1]);
            }
            int initialVertex = ParseLine()[0];
            return Solve(Enumerable.Range(0, Nodes.Length).ToHashSet()).Item1.Contains(initialVertex) ? 1 : 0;
        }

        private (Game, Game) Solve(Game g) {
            if (!g.Any()) {
                return (EmptyGame, EmptyGame);
            }
            int p = Nodes.Where((_, i) => g.Contains(i)).Max(x => x.Color);
            var u = new Game();
            for (int idx = 0; idx < Nodes.Length; ++idx) {
                if (g.Contains(idx) && Nodes[idx].Color == p) u.Add(idx);
            }
            int player = p % 2;
            var a = Attractor(g, u, player);
            var res = Solve(g.Except(a).ToHashSet());
            if (!res.Select(1 - player).Any()) {
                return (player == 0)? (a.Union(res.Item1).ToHashSet(), EmptyGame) : (EmptyGame, a.Union(res.Item2).ToHashSet());
            }
            var b = Attractor(g, res.Select(1 - player), 1 - player);
            var res2 = Solve(g.Except(b).ToHashSet());
            res2.Select(1 - player).UnionWith(b);
            return res2;
        }

        private Game Attractor(Game g, Game v, int player) {
            Game res = new Game(v);
            bool wasAdded;
            do {
                wasAdded = false;
                foreach(int node in g.Except(res)) {
                    if (Nodes[node].Player == 0 && Edges[node].Any(x => res.Contains(x))) {
                        wasAdded = true;
                        res.Add(node);
                    } else if (Nodes[node].Player == 1 && Edges[node].All(x => res.Contains(x))) {
                        wasAdded = true;
                        res.Add(node);
                    }
                }
            } while (wasAdded);
            return res;
        }
    }
}
