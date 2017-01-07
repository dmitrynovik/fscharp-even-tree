namespace FSharp.EvenTree

[<CustomEquality; NoComparison>]
type Node = { id: int; edges: List<Node> } with

    override this.GetHashCode() = this.id

    override x.Equals(obj) =
        match obj with
            | :? Node as y -> x.id = y.id
            | _ -> false

    member this.AddEdge(n: Node): Node =
        if List.exists (fun n2 -> n2 = n) this.edges then this
        else { id = this.id; edges = n :: this.edges }

    member this.RemoveEdge(n: Node): Node = 
        { id = this.id; edges = List.filter (fun n2 -> n2.id <> n.id) this.edges }        


type Forest = { roots: List<Node> }


