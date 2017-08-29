using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matchmaker : MonoBehaviour {

    Player p0 = null;
    double Tao = 0.5;
    // Use this for initialization
    void Start () {


		Player test1 = new Player ();
		Player test2 = new Player ();
		Player test3 = new Player ();
        Player test4 = new Player ();

        //results 1 = player 1 won, 0 = opponent won

        
      


        // Main Player
		test1.setId (0);
		test1.setRanked (true);
		test1.setRating (1500);
        test1.setDeviation(200);
        test1.setVolatility (0.06);
		
        // opponents
		test2.setId (1);
		test2.setRanked (true);
        test2.setRating(1400);
        test2.setDeviation(30);
        test2.setVolatility(0.06);
        test2.setResults(1);

		test3.setId (2);
		test3.setRanked (true);
        test3.setRating(1550);
        test3.setDeviation(100);
        test3.setVolatility(0.06);
        test3.setResults(0);

        test4.setId(3);
        test4.setRanked(true);
        test4.setRating(1700);
        test4.setDeviation(300);
        test4.setVolatility(0.06);
        test4.setResults(0);

        
    
		List<Player> players = new List<Player>();
		players.Add (test1);
		players.Add (test2);
		players.Add (test3);
        players.Add(test4);
       
		updateRanking (players);
	}

    void initializePlayer(Player p)
    {
        // if the player is unrated use a rating of 1500  set the volatility to 0.6 ( this could be changed not exactly sure how yet)

        if (!p.getRanked()) { 
        setRating(p);
        setRatingVolatility(p);
        setRatingDeviation(p);
         }
	
	}

	void setRating(Player p){
		if (!p.getRanked()) {
			p.setRating (1500);
		}
	
	}

	void setRatingDeviation(Player p){
		if (!p.getRanked ()) {
			p.setDeviation (350);
		
		}
	
	}

	void setRatingVolatility(Player p){
		if (!p.getRanked()) {
			p.setVolatility (0.6);
		
		}
	}



	void updateRanking(List<Player> players){
        // steps to update Ranking they are based on the Glicko-2 steps.


        // Symbols that are used and what they mean
        // Tao = system constant that constrains change in volatility over time
        //Tao reasonably exists between 0.3 and 1.2  for initial testing i will be using 0.5  // if the results of the particular game
        // are expected to vary wildly then the lower the number the better ( i.e. more RNG use a lower #)
        // Rating is represented in the forumla's as µ (Mu)
        // Rating Deviation is represented as ϕ (Phi)
        // The estimated improvement based on results alone is represented as ∆ (Delta)
        // The convergence tolerance is represented as ε (Epsilon)


        p0 = players[0]; // main player





        // step one and two  initialize all players and convert Ratings and deviation to Glicko-2


        foreach (Player p in players)
		{
			initializePlayer (p);


			p.setRating ((p.getRating () - 1500) / 173.7178);
			p.setDeviation((p.getDeviation()/173.7178));
			//Debug.Log ("id" + p.getId() +" ranked " + p.getRanked() + " rating: " + p.getRating() + " deviation: " +p.getDeviation() + " g(ϕ): " + calculateG(p.getDeviation()) + " E: " + calculateE(p.getDeviation(),p.getRating())) ;
            
        }

        //Step 3 calculate the estimated Variance
     
        double variance = calculateVariance(players);
        
        // Step 4 calculate Delta
        double delta = calculateDelta(variance,players);
        //Debug.Log("variance = " + v + " delta = " + delta);

        // Step 5 calculate volatility
        double a = System.Math.Log(System.Math.Pow(p0.getVolatility(), 2));
        double vol = calculateVolatility(delta,variance,a);
        Debug.Log(vol);

        //Step 6 update rating deviation to pre-rating period value
        double preRating = calculatePreRating(vol);
        Debug.Log(preRating);

        //step 7 update rating and deviation to new values
        double glickoScaleRating, glickoScaleDeviation;
        glickoScaleDeviation = calculateDeviation(preRating, variance);
        glickoScaleRating = calculateRating(glickoScaleDeviation, players);

        //step 8 convert to original scale
        double originalScaleRating, originalScaleDeviation;
        originalScaleDeviation = convertDeviationToOriginal(glickoScaleDeviation);
        originalScaleRating = convertRatingsToOriginal(glickoScaleRating);

        Debug.Log("prerating: " + preRating + " gSD: " + glickoScaleDeviation + " gSR: " + glickoScaleRating + " oSD: " + originalScaleDeviation + " oSR: " + originalScaleRating);




    }

	double calculateE(double rd, double rating){

        return (1 / (1 + System.Math.Exp(-1 * calculateG(rd) * (p0.getRating() - rating))));
		
	}
    
	double calculateG(double rd){
		// gPhi = 	1/((1+3Phi^2)/pi^2)
			
		return 1/System.Math.Sqrt(1 + 3*System.Math.Pow(rd,2)/System.Math.Pow(System.Math.PI,2));

			}
    double calculateVariance(List<Player> players) {
        double tempSum = 0.0;
        for (int i = 1; i < players.Count; i++)
        {
            double tempE = calculateE(players[i].getDeviation(), players[i].getRating());
            tempSum += System.Math.Pow(calculateG(players[i].getDeviation()), 2)*tempE*(1-tempE);
        }


        return 1/tempSum;


    }
    double calculateDelta(double v,List<Player> players) {

        
        double tempSum = 0;
        for (int i = 1; i < players.Count; i++) {
           // Debug.Log("pid"+ players[i].getId() +"G : " + calculateG(players[i].getDeviation()) + " s = " + players[i].getResults() + " E  = " + calculateE(players[i].getDeviation(), players[i].getRating()));
           tempSum += calculateG(players[i].getDeviation()) * (players[i].getResults() - calculateE(players[i].getDeviation(),players[i].getRating()));
        }
        return v*tempSum;
    }

    double calculateVolatility(double delta, double v, double a) {
       
        double A = a;
        double B,k;
        double dev = p0.getDeviation();
        double epsilon = 0.0000001;
        if (System.Math.Pow(delta, 2) > System.Math.Pow(dev, 2) + v)
        {
            B = System.Math.Log(System.Math.Pow(delta, 2) - System.Math.Pow(dev, 2) - v);
        }
        else {
            k = 1;
            double x = (A - k * Tao);
            while (findFx(delta,v,A,x)<0) {
                k++;
            }
            B = (A - k * Tao);

        }
        double fA = findFx(delta, v, a, A);
        double fB = findFx(delta, v, a, B);
        double C, fC;
        while (System.Math.Abs(B-A)>epsilon) {
           
            C = A + (A - B) * fA/(fB-fA);
            fC = findFx(delta, v, a, C);
            if ((fC * fB) < 0)
            {
                A = B;
                fA = fB;
            }
            else
            {
                fA = fA / 2;

            }
            B = C;
            fB = fC;

           
        }
        
        return System.Math.Exp(A / 2);


        
       
    }

    double findFx(double delta, double v, double a, double x) {
        
        double dev = p0.getDeviation();
        double LeftEq = (System.Math.Exp(x) * ( System.Math.Pow(delta,2) - System.Math.Pow(dev,2) - v - System.Math.Exp(x)))/(2*System.Math.Pow(System.Math.Pow(dev,2)+v+System.Math.Exp(x),2));
        double RightEq = (x - a)/System.Math.Pow(Tao,2);
        return LeftEq-RightEq;
    }
    double calculatePreRating(double volatility) {
        return System.Math.Sqrt(System.Math.Pow(p0.getDeviation(), 2) + System.Math.Pow(volatility, 2));
            }
    double calculateDeviation(double preRating, double variance) {
        return 1 / System.Math.Sqrt(1 / System.Math.Pow(preRating, 2) + 1 / variance);
        
    }
    double calculateRating(double updatedDeviation,List<Player> players) {
       
        double tempSum = 0.0;
        for (int i = 1; i < players.Count; i++) {
           tempSum += calculateG(players[i].getDeviation()) * (players[i].getResults() - calculateE(players[i].getDeviation(), players[i].getRating()));
        }
        return System.Math.Pow(updatedDeviation, 2) * tempSum + p0.getRating();
    }
    double convertRatingsToOriginal(double rating) {
        return 173.7178 * rating + 1500;

    }
    double convertDeviationToOriginal(double deviation) {
        return 173.7178 * deviation;
    }
	// Update is called once per frame
	void Update () {
		
	}
}

public class Player
{
					double _Id;

	double _rating;
	double _volatility;
	double _deviation;
	bool _ranked;
    int _results;
	public Player ()
	{


	}
 
	public double getId(){

        return _Id;
    }
	public double getRating ()
	{
		return _rating;
	}
	public double getVolatility (){
		return _volatility;
	}
	public double getDeviation (){
		return _deviation;
	}
	public bool getRanked(){
		return _ranked;
	}
    public int getResults() {
        return _results;
    }

    public void setId(double Id){
		_Id = Id;
    }
	public void setRating(double r){
	
		_rating = r;
	
	}
	public void setVolatility(double v){

		_volatility = v;

	}
	public void setDeviation(double d){

		_deviation = d;

	}
	public void setRanked(bool rnk){

		_ranked = rnk;

	}
    public void setResults(int results) {
        _results = results;
    }
}
