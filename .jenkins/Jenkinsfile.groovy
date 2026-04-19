pipeline {
	agent {
		label '(linux && docker) || (windows && unity)'
	}

	options {
		disableConcurrentBuilds()
		disableResume()
	}

	stages {
		stage('Read unityProject.properties') {
			steps {
				script {
					def unityConfig = readProperties file: '.jenkins/unityProject.properties'

					if (isUnix()) {
						docker.image('faulo/compose-unity:latest').inside {
							unityProject(unityConfig)
						}
					} else {
						unityProject(unityConfig)
					}
				}
			}
		}
	}
}