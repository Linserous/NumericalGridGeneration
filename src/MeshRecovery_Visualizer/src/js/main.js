var testGraphJson = {
  "edges": [{ id: 'n1', source: 'n1', target: 'n2' },
  { id: 'n2', source: 'n1', target: 'n3' },
  { id: 'n3', source: 'n2', target: 'n3' },
  { id: 'n4', source: 'n4', target: 'n5' }],
  "nodes": [{ id: 'n1', label: '1', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n2', label: '2', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n3', label: '3', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n4', label: '4', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n5', label: '5', size: 10, x: Math.random(), y: Math.random() }]
};
var loadTemplate = true;

window.onload = function (e) {
  if (loadTemplate) loadGraph();
};

function init() {
  loadTemplate = false;
}

function loadGraph() {
  var graph = loadTemplate ? testGraphJson : JSON.parse(arguments[0]);
  console.log(preprocess(graph));
  var Sigma = new sigma({
    graph: preprocess(graph),
    container: 'graph-container',
    settings: {
      defaultNodeColor: '#006699'
    }
  });
  // Initialize the dragNodes plugin
  var dragListener = new sigma.plugins.dragNodes(Sigma, Sigma.renderers[0]);
}


// private functions, which use loadGraph
function preprocess(graph) {
  graph.nodes.forEach(function (el) {
    el.x = Math.random();
    el.y = Math.random();
    el.size = 15;
  }, this);
  return graph;
}