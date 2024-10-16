# Firefly Backend Test

Web API with Entity Framework CRUD operations. Microsoft SQL Server DB deployed in Docker. TestContainers running the SQL Server as part of integration tetsts. Containerised in Docker and Deploying via GitHub actions to AKS. 

## Development Tips

### Steps to run app locally:
- run SQL Server DB on docker via command `docker compose up -d`
- run migrations to set up DB tables via command `dotnet ef database update`
	- migration is now being run automatically by code in Program.cs
- Press f5 to run https launch setting


### Deployment to AKS:

Ensure kubectl and helm are configure to point at cluster by commands:
- `az account set --subscription bd16107c-feba-42e3-9cde-d1fdf22dc948`
- `az aks get-credentials --resource-group morganmwj-rg-development --name morganmwj-aks-development --overwrite-existing`

To deploy run `helm upgrade --install firefly-release .\api-helm-chart\ -n firefly --set image.tag=<latest-acr-image-tag>`

### Monitoring via K8s Dashboard:

Install kubernetes dashboard:
```
helm repo add kubernetes-dashboard https://kubernetes.github.io/dashboard/ \
helm upgrade --install kubernetes-dashboard kubernetes-dashboard/kubernetes-dashboard --create-namespace --namespace kubernetes-dashboard
```

Run `kubectl -n kubernetes-dashboard port-forward svc/kubernetes-dashboard-kong-proxy 8443:443 &`  \
This allows access to the k8s dashboard through [localhost:8443](http://localhost:8443) by forwarding traffic from port 8443 to port 443 on the service (kubernetes-dashboard-kong-proxy) in the cluster.

The dashboard requires and access/bearer token this can be generated from a service account with correct permissions.\
Run `kubectl -n kubernetes-dashboard create token k8s-dashboard-admin-user` to generate this.


Here is manifest code to set up service account with correct permissions:
```
apiVersion: v1
kind: ServiceAccount
metadata:
  name: k8s-dashboard-admin-user
  namespace: kubernetes-dashboard

apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: k8s-dashboard-admin-user
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: ClusterRole
  name: cluster-admin # this references a built in ClusterRole with admin permissions
subjects:
- kind: ServiceAccount
  name: k8s-dashboard-admin-user
  namespace: kubernetes-dashboard
```

![K8s Dashboard](resources/k8s_dashboard.png)
*The kubernetes Dashboard screen shot*

## Issues

- Cannot access api once deployed
- No logging to check if health live/readiness probes are working

## Improvements

- Better logging of events and http request/responses
- Perhaps expose API via Azure APIM?

## Original Requirements:

The intention of this test is to provide an API that allows a consumer to create a basic class structure containing students and teachers.

1. Complete the API controllers to provide creation and retrieval for Classes, Students and Teachers.
2. Complete the methods AssignClass in the TeachersController and EnrollStudents in the ClassesController.
3. Add data validation for the StudentDto, TeacherDto and ClassDto so that the following holds:
	- A Class must have a name which is a maximum of 20 characters
	- A Class must have a capacity which is a minimum of 5 and a maximum of 30
	- A Teacher must have a name which is a maximum of 50 characters
	- A Student must have a name which is a maximum of 50 characters
4. Add business rule validation so that:
	- A student cannot be enrolled in a class that is over its capacity
	- A teacher cannot be assigned to more that 5 classes
	- Only one teacher may be assigned per class
5. Provide unit tests for the validations detailed in step 3