        // This was originally a coderbyte.com task.
        // I encountered this question in a code assessment and couldn't do it at the time.
        // I can't guarantee all coderbyte.com checks will be correct because i didn't pay for the site .d
        // Since its only for paid users, i got this example from 
        // chegg.com/homework-help/questions-and-answers/write-python-function-citytraffic-strarr-read-strarr-representation-undirected-graph-form--q45833346.
        private class Graph
        {
            private readonly Dictionary<int, List<int>> _adjacencyDict = new();

            public List<int> CitiesOrdered => _adjacencyDict.Keys.OrderBy(cityPop => cityPop).ToList();

            public void AddEdge(int v, int w)
            {
                if (_adjacencyDict.TryGetValue(v, out var adjacencyList))
                {
                    adjacencyList.Add(w);
                }
                else
                {
                    _adjacencyDict.Add(v, new List<int> { w });
                }
            }

            public int GetMaxTrafficForCity(int targetCity)
            {
                int maxTraffic = 0;

                foreach (int city in _adjacencyDict.Keys.Where(city => city != targetCity))
                {
                    maxTraffic = Math.Max(maxTraffic, BfsCityToCityTraffic(city, targetCity));
                }

                return maxTraffic;
            }

            private int BfsCityToCityTraffic(int fromCity, int targetCity)
            {
                // We need this because if we get to targetCity only one way, we have to calculate all nodes.
                bool targetIsOneWay = _adjacencyDict[targetCity].Count == 1;

                int traffic = fromCity;

                List<int> visitedCities = new() { fromCity };

                Queue<int> toBeVisitedCities = new();
                toBeVisitedCities.Enqueue(fromCity);

                while (toBeVisitedCities.Count != 0)
                {
                    int currentCity = toBeVisitedCities.Dequeue();

                    // Get neighbourCities that visitedCities doesn't contain.
                    List<int> neighbourCities = _adjacencyDict[currentCity].Where(city => !visitedCities.Contains(city)).ToList();

                    int indexOfTarget = neighbourCities.IndexOf(targetCity);
                    // If neighbourCities has targetCity, we have to iterate targetCity last because
                    // we want other neighbours to add their population to traffic.
                    if (indexOfTarget != -1 && indexOfTarget != neighbourCities.Count - 1)
                    {
                        int lastItem = neighbourCities.Last();
                        neighbourCities[neighbourCities.Count - 1] = targetCity;
                        neighbourCities[indexOfTarget] = lastItem;
                    }

                    foreach (int neighbourCity in neighbourCities)
                    {
                        bool neighIsTarget = neighbourCity == targetCity;
                        
                        // If target has only one way to reach, don't interrupt while loop and just calculate all nodes.
                        switch (neighIsTarget)
                        {
                            case true when !targetIsOneWay:
                                return traffic;
                            case false:
                                traffic += neighbourCity;
                                break;
                        }

                        toBeVisitedCities.Enqueue(neighbourCity);
                    }

                    visitedCities.AddRange(neighbourCities);
                }

                return traffic;
            }
        }

        private static string GetMaxTrafficByCity()
        {
            string[] strArray = { "1:[5]", "4:[5]", "3:[5]", "5:[1,4,3,2]", "2:[5,15,7]", "7:[2,8]", "8:[7,38]", "15:[2]", "38:[8]" };

            Graph graph = new();

            foreach (string s in strArray)
            {
                string[] splitCityAndNeighbours = s.Split(':');
                int cityPopulation = int.Parse(splitCityAndNeighbours[0]);

                string[] splitNeighbours = splitCityAndNeighbours[1].Split(',');
                foreach (string splitNeighbour in splitNeighbours)
                {
                    string fooSplitNeighbour = splitNeighbour.Replace("[", string.Empty).Replace("]", string.Empty);
                    if (int.TryParse(fooSplitNeighbour, out int splitNeighbourNumber))
                    {
                        graph.AddEdge(cityPopulation, splitNeighbourNumber);
                    }
                }
            }

            string citiesAndTheirMaxTraffics = "";
            List<int> citiesOrdered = graph.CitiesOrdered;
            for (int i = 0; i < strArray.Length; i++)
            {
                int cityPop = citiesOrdered[i];
                citiesAndTheirMaxTraffics += $"{cityPop}:{graph.GetMaxTrafficForCity(cityPop)}";

                if (i != strArray.Length - 1)
                {
                    citiesAndTheirMaxTraffics += ",";
                }
            }

            return citiesAndTheirMaxTraffics;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine(GetMaxTrafficByCity());
        }