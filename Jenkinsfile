pipeline {
    agent any

    stages {
        stage('Clone') {
            steps {
                git 'https://github.com/OmarAlfar0uk/UniversityManagementSystemMicroservice.git'
            }
        }

        stage('Build') {
            steps {
                sh 'docker compose build'
            }
        }

        stage('Run') {
            steps {
                sh 'docker compose up -d'
            }
        }
    }
}
