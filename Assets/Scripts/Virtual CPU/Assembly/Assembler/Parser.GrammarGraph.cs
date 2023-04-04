using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Utilities;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public partial class Parser
    {
        public class GrammarGraph
        {
            public GrammarNode Root { get; }

            private GrammarGraph()
            {
                Root = new GrammarNode(null);
            }

            public class GrammarNode
            {
                public NodePattern Pattern { get; }
                public bool IsFinal { get; internal set; }
                public AbstractStatement Statement { get; internal set; }
                public Type StatementType { get; internal set; }
                public List<GrammarNode> Children { get; }

                public GrammarNode(NodePattern pattern)
                {
                    Pattern = pattern;
                    Children = new List<GrammarNode>();
                    IsFinal = false; 
                }

                public GrammarNode(NodePattern pattern, AbstractStatement stmt)
                {
                    Pattern = pattern;
                    Children = new List<GrammarNode>();
                    Statement = stmt;
                    IsFinal = true;
                }

                private bool Equals(GrammarNode other)
                {
                    return Equals(Pattern, other.Pattern);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    return obj.GetType() == GetType() && Equals((GrammarNode)obj);
                }

                public override int GetHashCode()
                {
                    return Pattern != null ? Pattern.GetHashCode() : 0;
                }
            }

            // TODO: Run topological sort on the statements (to resolve IntermediateStatement dependencies)
            private static List<AbstractStatement> FindAllStatementTypes()
            {
                AbstractStatement CreateInstance(Type t)
                {
                    return (AbstractStatement)Activator.CreateInstance(t, new object[] {null});
                }

                List<AbstractStatement> stmts = System.Reflection.Assembly.GetAssembly(typeof(AbstractStatement)).GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSealed && type.IsSubclassOf(typeof(AbstractStatement)))
                    .Select(CreateInstance)
                    .ToList(); 

                // TODO: Quick hacky way to get directives working, replace with topo sort
                stmts.Sort((s1, s2) => (s1, s2) switch
                {
                    (IntermediateStatement, IntermediateStatement) => 0,
                    (FinalStatement, FinalStatement) => 0,
                    (IntermediateStatement, FinalStatement) => 1,
                    (FinalStatement, IntermediateStatement) => -1,
                    _ => 0
                });

                return stmts;
            }
                
            
            private void AddPath(AbstractStatement stmt, List<Type> connectTo = null)
            {
                Queue<NodePattern> patterns = new(stmt.Pattern.Select(p => p.pattern));
                GrammarNode currentNode = Root;
                while (patterns.Count > 0)
                {
                    NodePattern pattern = patterns.Dequeue();

                    GrammarNode node = 
                        patterns.Count == 0 
                            ? new GrammarNode(pattern, stmt) 
                            : new GrammarNode(pattern);
                    
                    if (pattern.Cycle)
                        node.Children.Add(node);

                    if (patterns.Count == 0 && connectTo is not null)
                    {
                        currentNode.Children.Add(node);
                        
                        // CONSIDER: Take most derived common base of the followedBy types
                        // and if it isn't AbstractStatement, find earliest node based on that?
                        
                        // Actually a bit of a dirty hack, as this would potentially allow IntermediateStatements
                        // to be followed by statements that are not specified in followedBy
                        
                        // This could be better - IntermediateStatements could just require the statement that follows
                        // to be of one of the types specified in followedBy

                        Type mostCommonType = TypeUtils.MostDerivedCommonBase(connectTo);
                        GrammarNode earliest = FindEarliestOfType(Root, mostCommonType);
                        if (earliest is null)
                            throw new InternalErrorException($"Earliest node for {mostCommonType} not found");
                            
                        node.Children.Add(earliest);

                        // foreach (Type type in connectTo)
                        // {
                        //     GrammarNode earliest = FindEarliestOfType(Root, type);
                        //     if (earliest is null)
                        //         throw new InternalErrorException($"Earliest node for {type} not found");
                        //         
                        //     node.Children.Add(earliest);
                        // }
                    }
                    else
                    {
                        GrammarNode child = currentNode.Children.Find(n => n.Equals(node));
                        if (child == null)
                        {
                            currentNode.Children.Add(node);
                            currentNode = node;
                        }
                        else
                        {
                            child.IsFinal |= node.IsFinal;
                            child.Statement ??= node.Statement;
                            child.StatementType ??= node.StatementType;
                            currentNode = child;
                        }
                    }
                }
            }

            private void UpdateBaseTypes(GrammarNode node)
            {
                if (node.Children.Count(n => !Equals(n, node)) == 0)
                    node.StatementType = node.Statement.GetType();
                else
                {
                    foreach (GrammarNode child in node.Children)
                    {
                        if (!Equals(child, node))
                            UpdateBaseTypes(child);
                    }

                    if (!Equals(node, Root))
                    {
                        List<Type> types = node.Children.Select(child => child.StatementType).ToList();
                        node.StatementType = TypeUtils.MostDerivedCommonBase(types);
                        if (node.Statement is not null)
                        {
                            types.Add(node.Statement.GetType());
                            node.StatementType = TypeUtils.MostDerivedCommonBase(types);
                        }
                    }
                        
                }
            }

            private static GrammarNode FindEarliestOfType(GrammarNode node, Type type)
            {
                if (node.Children.Count(n => !Equals(n, node)) == 0) return null;
                if (node.StatementType == type) return node;
                foreach (GrammarNode child in node.Children)
                {
                    if (!Equals(child, node))
                    {
                        GrammarNode potentialMatch = FindEarliestOfType(child, type);
                        if (potentialMatch is not null) return potentialMatch;
                    }
                }

                return null;
            }
            
            public static GrammarGraph Build()
            {
                GrammarGraph graph = new();

                List<AbstractStatement> statements = FindAllStatementTypes();
                foreach (var stmt in statements)
                {
                    switch (stmt)
                    {
                        case FinalStatement s:
                            graph.AddPath(s);
                            break;
                        case IntermediateStatement s:
                            graph.UpdateBaseTypes(graph.Root);
                            graph.AddPath(s, s.FollowedBy);
                            break;
                    }
                }
                
                return graph;
            }
        }
    }
}