using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Graph;

namespace Helpers;
public class Graph(IEnumerable<Node> nodes, IEnumerable<Edge> edges) {
	public record Node(string Id, string Label);
	public record Edge(string Id, string SourceId, string TargetId, string Label);
	public void WriteToHtml() {
		var path = DateTime.Now.ToString("yyyyMMdd-hhmsfff");
		Directory.CreateDirectory(path);
		File.Copy("../../../../graph-cytoscape/cytoscape.min.js", Path.Combine(path, "cytoscape.min.js"));
		var htmlPath = Path.Combine(path, "index.html");
		//{ data: { id: 'a', label: 'Node A' } },
		//{ data: { id: 'ab', source: 'a', target: 'b', label: 'A→B' } },
		StringBuilder elements = new StringBuilder();
		foreach (var node in nodes) elements.AppendLine($"{{ data: {{ id: '{node.Id}', label: '{node.Label}' }} }},");
		foreach (var edge in edges) elements.AppendLine($"{{ data: {{ id: '{edge.Id}', source: '{edge.SourceId}', target: '{edge.TargetId}', label: '{edge.Label}' }} }},");
		File.WriteAllText(htmlPath, $$"""

			<!doctype html>
			<html lang=en>

			<head>
			    <meta charset=utf-8>
			    <title>Cytoscape.js Graph Visualization</title>
			    <style>
			        body {
			            font-family: Arial, sans-serif;
			            margin: 0;
			            padding: 20px;
			            background-color: #f5f5f5;
			        }

			        #cy {
			            width: 100%;
			            height: 600px;
			            border: 1px solid #ccc;
			            background-color: white;
			            border-radius: 8px;
			            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
			        }

			        .controls {
			            margin-bottom: 20px;
			        }

			        button {
			            margin: 5px;
			            padding: 10px 15px;
			            border: none;
			            border-radius: 4px;
			            background-color: #007bff;
			            color: white;
			            cursor: pointer;
			        }

			        button:hover {
			            background-color: #0056b3;
			        }
			    </style>
			</head>

			<body>
			    <h1>Graph Visualization with Cytoscape.js</h1>

			    <div class="controls">
			        <button onclick="changeLayout('grid')">Grid Layout</button>
			        <button onclick="changeLayout('circle')">Circle Layout</button>
			        <button onclick="changeLayout('cose')">Force Layout</button>
			    </div>

			    <div id="cy"></div>

			    <script src="cytoscape.min.js"></script>
			    <script>
			        var cy = cytoscape({
			            container: document.getElementById('cy'),

			            elements: [
			               {{elements.ToString()}}
			            ],

			            style: [
			                {
			                    selector: 'node',
			                    style: {
			                        'background-color': '#4CAF50',
			                        'label': 'data(label)',
			                        'text-valign': 'center',
			                        'text-halign': 'center',
			                        'color': 'white',
			                        'font-size': '14px',
			                        'font-weight': 'bold',
			                        'width': '60px',
			                        'height': '60px',
			                        'border-width': 2,
			                        'border-color': '#2E7D32'
			                    }
			                },
			                {
			                    selector: 'node:selected',
			                    style: {
			                        'background-color': '#FF5722',
			                        'border-color': '#D84315',
			                        'border-width': 3
			                    }
			                },
			                {
			                    selector: 'edge',
			                    style: {
			                        'width': 3,
			                        'line-color': '#666',
			                        'target-arrow-color': '#666',
			                        'target-arrow-shape': 'triangle',
			                        'curve-style': 'bezier',
			                        'label': 'data(label)',
			                        'font-size': '12px',
			                        'text-rotation': 'autorotate',
			                        'text-margin-y': -10
			                    }
			                },
			                {
			                    selector: 'edge:selected',
			                    style: {
			                        'line-color': '#FF5722',
			                        'target-arrow-color': '#FF5722',
			                        'width': 5
			                    }
			                }
			            ],

			            layout: {
			                name: 'cose',
			                idealEdgeLength: 100,
			                nodeOverlap: 20,
			                refresh: 20,
			                fit: true,
			                padding: 30,
			                randomize: false,
			                componentSpacing: 100,
			                nodeRepulsion: 400000,
			                edgeElasticity: 100,
			                nestingFactor: 5,
			                gravity: 80,
			                numIter: 1000,
			                initialTemp: 200,
			                coolingFactor: 0.95,
			                minTemp: 1.0
			            },

			            // Enable zoom and pan
			            minZoom: 0.1,
			            maxZoom: 3,
			            wheelSensitivity: 0.1
			        });

			        // Add some interactive features
			        cy.on('tap', 'node', function (evt) {
			            var node = evt.target;
			            console.log('Tapped node:', node.id());
			        });

			        cy.on('tap', 'edge', function (evt) {
			            var edge = evt.target;
			            console.log('Tapped edge:', edge.id());
			        });

			        function changeLayout(layoutName) {
			            var layoutOptions = {
			                name: layoutName,
			                fit: true,
			                padding: 30
			            };

			            if (layoutName === 'grid') {
			                layoutOptions.rows = Math.ceil(Math.sqrt(cy.nodes().length));
			            }

			            cy.layout(layoutOptions).run();
			        }

			        function clearGraph() {
			            if (confirm('Are you sure you want to clear the graph?')) {
			                cy.elements().remove();
			            }
			        }

			        // Fit the graph to the container when it's ready
			        cy.ready(function () {
			            cy.fit();
			        });
			    </script>
			</body>

			</html>

			""");
		Process.Start(path);
	}
}