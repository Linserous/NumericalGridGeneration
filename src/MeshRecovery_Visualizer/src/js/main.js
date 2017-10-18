var Sigma;

window.onload = function (e) {
  // for testing in Browser, please uncomment following lines:
  // var json = {
  //   "edges": [{ id: 'n1', source: 'n1', target: 'n2' },
  //   { id: 'n2', source: 'n1', target: 'n3' },
  //   { id: 'n3', source: 'n2', target: 'n3' },
  //   { id: 'n4', source: 'n4', target: 'n5' }],
  //   "nodes": [{ id: 'n1', label: '1', size: 10, x: Math.random(), y: Math.random() },
  //   { id: 'n2', label: '2', size: 10, x: Math.random(), y: Math.random() },
  //   { id: 'n3', label: '3', size: 10, x: Math.random(), y: Math.random() },
  //   { id: 'n4', label: '4', size: 10, x: Math.random(), y: Math.random() },
  //   { id: 'n5', label: '5', size: 10, x: Math.random(), y: Math.random() }]
  // };
  //  loadGraph(json);
};

function loadGraph(jsonStr) {
  var graph = JSON.parse(jsonStr);
  Sigma = new sigma({
    graph: graph,
    container: 'graph-container',
    settings: {
      defaultNodeColor: '#006699'
    }
  });
  // Initialize the dragNodes plugin
  var dragListener = new sigma.plugins.dragNodes(Sigma, Sigma.renderers[0]);
}