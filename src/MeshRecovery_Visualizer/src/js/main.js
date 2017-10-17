window.onload = function (e) {
  var i,
    s,
    N = 10,
    E = 10,
    g = {
      nodes: [],
      edges: []
    };
  // Generate a random graph:
  for (i = 0; i < N; ++i)
    g.nodes.push({
      id: 'n' + i,
      label: 'Node ' + i,
      x: Math.random(),
      y: Math.random(),
      size: Math.random(),
      color: 'green'
    });
  for (i = 0; i < E; ++i)
    g.edges.push({
      id: 'e' + i,
      source: 'n' + (Math.random() * N | 0),
      target: 'n' + (Math.random() * N | 0),
      size: Math.random(),
      color: '#ccc'
    });
  s = new sigma({
    graph: g,
    container: 'graph-container'
  });
};