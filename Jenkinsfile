pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                sh 'docker-compose build'
            }
        }

        stage('Deploy') {
            steps {
                sh 'docker-compose up -d'
            }
        }
    }

    post {
        success {
            echo 'Deployment completed successfully'
        }
        failure {
            echo 'Pipeline failed'
        }
    }
}
