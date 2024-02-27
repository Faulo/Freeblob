node {
							commitMessage = ""
							for ( changeLogSet in currentBuild.changeSets){
								for (entry in changeLogSet.getItems()){
									commitMessage += entry.msg + "\n"
								}
							}
							
							echo commitMessage
}