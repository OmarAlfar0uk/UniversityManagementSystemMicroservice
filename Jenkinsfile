pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                sh 'sudo docker-compose build'
            }
        }

        stage('Deploy') {
            steps {
                sh 'sudo docker-compose up -d'
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
