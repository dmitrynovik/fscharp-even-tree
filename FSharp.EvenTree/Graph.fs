namespace FSharp.EvenTree

type Node = int
    
[<CustomEquality; NoComparison>]
type Edge = { v1: Node; v2: Node } with

    override this.GetHashCode() = this.v1.GetHashCode() ^^^ this.v2.GetHashCode()

    override this.Equals(obj) = 
        match obj with
            | :? Edge as e -> (e.v1 = this.v1 && e.v2 = this.v2) || (e.v1 = this.v2 && e.v2 = this.v1)
            | _ -> false

//
// Immutable Graph type (each modification produces new graph):
//
type Graph(nodes: List<Node>, edges: List<Edge>) =

    static member ParseNode(sNode: string) = System.Int32.Parse sNode

    member this.Nodes = nodes
    member this.NodeCount = nodes.Length
    member this.Edges = edges

    member this.AddNode(n: Node): Graph =
        if List.contains n nodes then this
        else Graph(n :: nodes, edges)

    member this.AddEdge(e: Edge): Graph =
        if e.v1 = e.v2 then this // ignore malformed edges like an edge to itself
        elif List.contains e edges then this
        else Graph(this.AddNode(e.v1).AddNode(e.v2).Nodes, e :: edges)

    member this.RemoveEdge(e: Edge): Graph = Graph(nodes, List.except [e] edges)

    member this.Union(other: Graph):Graph = 
        let nodes = Seq.append nodes other.Nodes |> Seq.distinct |> Seq.toList
        let edges = Seq.append edges other.Edges |> Seq.distinct |> Seq.toList
        Graph(nodes, edges)

    member this.NeigborsOf(n: Node) = 
        let nEdges = List.filter (fun e -> e.v1 = n || e.v2 = n) edges
        seq { for e in nEdges do if e.v1 = n then yield e.v2 else yield e.v1 }
        
    member this.SubTree(root: Node):Graph = 
        let neighbors = this.NeigborsOf root
        let mutable g = Graph(List.Empty, List.Empty)
        if Seq.isEmpty neighbors then g
        else 
            for n in neighbors do
                let nTree = this.SubTree n
                g <- g.Union(nTree).AddEdge({v1 = root; v2 = n})        
            g

    member this.maxEvenDepth() =
        let root = List.min nodes
        let neighbors = this.NeigborsOf root
        let mutable count = 0
        for n in neighbors do
            let subTree = this.SubTree n
            if subTree.NodeCount % 2 = 0 then count <- count + subTree.maxEvenDepth() + 1
        count

    member this.parseFromStdin() = this.parse System.Console.ReadLine

    member this.parse(readLineFunc: unit -> string):Graph = 

        let mutable graph = Graph(List.Empty, List.Empty)

        // Lines lazy sequence:
        let lines = seq { 
            let mutable hasMore = true
            while hasMore do
                let line = readLineFunc()
                if line <> null then yield line
                else hasMore <- false
        }

        for line in Seq.skip 1 lines do // skip 1st line as irrelevant
            let nodes = (line.Split ' ') |> Seq.map Graph.ParseNode |> Seq.toArray
            graph <- graph.AddEdge { v1 = nodes.[0]; v2 = nodes.[1] }
            
        graph


