# fscharp-even-tree
https://www.hackerrank.com/challenges/even-tree

You are given a tree (a simple connected graph with no cycles). The tree has  nodes numbered from  to  and is rooted at node .

Find the maximum number of edges you can remove from the tree to get a forest such that each connected component of the forest contains an even number of vertices.

## Input Format

The first line of input contains two integers  and .  is the number of vertices, and  is the number of edges. 
The next  lines contain two integers  and  which specifies an edge of the tree.

## Constraints

Note: The tree in the input will be such that it can always be decomposed into components containing an even number of nodes.

## Output Format

Print the number of removed edges.

## Sample Input

10 9
2 1
3 1
4 3
5 2
6 1
7 2
8 6
9 8
10 8
## Sample Output

2
## Explanation

On removing edges (1,3)  and (1, 6) , we can get the desired result.
