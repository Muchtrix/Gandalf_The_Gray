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
            Console.WriteLine("Hello World!");
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
            Nodes.Initialize();
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
        }

        private (Game, Game) Solve(Game g) {
            int p = Nodes.Where((_, i) => g.Contains(i)).Max(x => x.Color);
            if (p == 0) {
                return (g, EmptyGame);
            }
            var u = Nodes.Where((v, i) => g.Contains(i) && v.Color == p).Select((_, i) => i).ToHashSet();
            int player = p % 2;
            var a = Attractor(g, u, player);
            var res = Solve(g.Except(a).ToHashSet());
            if (res.Select(player).SetEquals(g)) {
                return (player == 0)? (g, EmptyGame) : (EmptyGame, g);
            }
            var b = Attractor(g, res.Select(1 - player), 1 - player);
            var res2 = Solve(g.Except(b).ToHashSet());
            res2.Select(1 - player).UnionWith(b);
            return res2;
        }

        private Game Attractor(Game g, Game v, int player) {
            throw new NotImplementedException();
        }
    }
}
