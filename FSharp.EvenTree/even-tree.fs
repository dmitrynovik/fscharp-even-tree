﻿module FSharp.EvenTree

type Node = int
    
[<CustomEquality; NoComparison>]
type Edge = { v1: Node; v2: Node } with
    override this.GetHashCode() = this.v1.GetHashCode() ^^^ this.v2.GetHashCode()
    override this.Equals(obj) = 
        match obj with
            | :? Edge as e -> e.v1 = this.v1 && e.v2 = this.v2 || e.v1 = this.v2 && e.v2 = this.v1
            | _ -> false

// Immutable Graph type (each modification produces new graph):
type Graph(nodes: List<Node>, edges: List<Edge>) =
    static member ParseNode(sNode: string) = System.Int32.Parse sNode
    member this.Nodes = nodes
    member this.Edges = edges

    member this.addNode(n: Node) =
        if List.contains n nodes then this
        else Graph(n :: nodes, edges)

    member this.addEdge(e: Edge) =
        if e.v1 = e.v2 then this // ignore malformed edges like an edge to itself
        elif List.contains e edges then this
        else Graph(this.addNode(e.v1).addNode(e.v2).Nodes, e :: edges)

    member this.removeEdge(e: Edge) = Graph(nodes, List.except [e] edges)

    member this.union(other: Graph) = 
        let nodes = Seq.append nodes other.Nodes |> Seq.distinct |> Seq.toList
        let edges = Seq.append edges other.Edges |> Seq.distinct |> Seq.toList
        Graph(nodes, edges)

    member this.childrenOf(n: Node) = 
        let nEdges = List.filter (fun e -> e.v1 = n || e.v2 = n) edges
        seq { for e in nEdges do if e.v1 = n && e.v2 > n then yield e.v2 elif e.v1 > n then yield e.v1 }
        
    member this.subTree(root: Node) = 
        let children = this.childrenOf root
        let mutable g = Graph(List.Empty, List.Empty)
        if Seq.isEmpty children then g
        else 
            for n in children do
                let nTree = this.subTree n
                g <- g.union(nTree).addEdge({v1 = root; v2 = n})        
            g

    member this.maxEvenDepth() =
        if Seq.isEmpty nodes then 0
        else
            let root = List.min nodes
            let children = this.childrenOf root
            let mutable count = 0
            for n in children do
                let subTree = this.subTree n
                let nodeCount = subTree.Nodes.Length
                if nodeCount < 2 then count <- count
                elif nodeCount = 2 then count <- count + 1
                elif nodeCount % 2 = 0 then count <- count + subTree.maxEvenDepth() + 1
                else count <- count + subTree.maxEvenDepth()
            count

    static member parseFromStdin() = Graph.parse System.Console.ReadLine

    static member parse(nextLine: unit -> string):Graph = 
        let mutable graph = Graph(List.Empty, List.Empty)

        let lines = seq { // lines lazy sequence ...
            let mutable hasMore = true
            while hasMore do
                let line = nextLine()
                if line <> null then yield line
                else hasMore <- false
        }

        for line in Seq.skip 1 lines do // skip 1st line as irrelevant
            let nodes = (line.Trim().Split ' ') |> Seq.map Graph.ParseNode |> Seq.toArray
            graph <- graph.addEdge { v1 = nodes.[0]; v2 = nodes.[1] }
            
        graph

[<EntryPoint>]
let main args = 
    let graph = Graph.parseFromStdin()
    let depth = graph.maxEvenDepth()
    System.Console.WriteLine depth
    //System.Console.Read() |> ignore
    0

