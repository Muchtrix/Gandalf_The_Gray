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
    }

    class ParityGameHelper {
        (int Color, int Player)[] Nodes;
        List<int>[] Edges;

        private static Game EmptyGame => new Game();

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
            var res = Solve(Enumerable.Range(0, Nodes.Length).ToHashSet());
            return res.Item1.Contains(initialVertex) ? 0 : 1;
        }

        private (Game, Game) Solve(Game g) {
            if (!g.Any()) {
                return (EmptyGame, EmptyGame);
            }
            int m = g.Max(x => Nodes[x].Color);
            var u = g.Where(x => Nodes[x].Color == m).ToHashSet();
            int player = m % 2;
            var a = Attractor(g, u, player);
            var res = Solve(g.Except(a).ToHashSet());
            var wnp = (player == 0) ? res.Item2 : res.Item1;
            if (!wnp.Any()) {
                return (player == 0)? (res.Item1.Union(a).ToHashSet(), EmptyGame) : (EmptyGame, res.Item2.Union(a).ToHashSet());
            }
            var b = Attractor(g, wnp, 1 - player);
            res = Solve(g.Except(b).ToHashSet());
            return (player == 0)? (res.Item1, res.Item2.Union(b).ToHashSet()) : (res.Item1.Union(b).ToHashSet(), res.Item2);
        }

        private Game Attractor(Game g, Game v, int player) {
            Game res = new Game(v);
            bool wasAdded;
            do {
                wasAdded = false;
                foreach(int node in g.Except(res)) {
                    if (Nodes[node].Player == player && Edges[node].Any(x => res.Contains(x))) {
                        wasAdded = true;
                        res.Add(node);
                    } else if ((Nodes[node].Player == 1 - player) && Edges[node].All(x => res.Contains(x))) {
                        wasAdded = true;
                        res.Add(node);
                    }
                }
            } while (wasAdded);
            return res;
        }
    }
}
